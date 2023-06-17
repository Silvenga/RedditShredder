using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using MediatR;
using Nito.AsyncEx;

namespace RedditShredder
{
    public class WorkQueue
    {
        private readonly Channel<INotification> _channel = Channel.CreateUnbounded<INotification>();
        private readonly AsyncLock _readLock = new();
        private readonly HashSet<INotification> _pending = new();

        public ValueTask Push(INotification notification, CancellationToken cancellationToken)
        {
            return _channel.Writer.WriteAsync(notification, cancellationToken);
        }

        public ValueTask Push(INotification notification)
        {
            return _channel.Writer.WriteAsync(notification);
        }

        public async ValueTask<NotificationReservation> Take(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _channel.Reader.WaitToReadAsync(cancellationToken);

                using (await _readLock.LockAsync(cancellationToken))
                {
                    if (_channel.Reader.TryRead(out var notification))
                    {
                        _pending.Add(notification);
                        return new NotificationReservation(notification, MarkCompleted, Push);
                    }
                }
            }

            throw new TaskCanceledException();
        }

        private async ValueTask MarkCompleted(INotification notification)
        {
            using (await _readLock.LockAsync())
            {
                _pending.Remove(notification);
            }
        }

        public async Task WaitForCompletion(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (await _readLock.LockAsync(cancellationToken))
                {
                    var completed = _pending.Count == 0
                                    && _channel.Reader.Count == 0;
                    if (completed)
                    {
                        return;
                    }
                }

                await Task.Delay(100, cancellationToken);
            }
        }
    }

    public class NotificationReservation : IAsyncDisposable
    {
        private readonly Func<INotification, ValueTask> _onSuccess;
        private readonly Func<INotification, ValueTask> _onFailure;
        private bool _committed;

        public INotification Notification { get; }

        public NotificationReservation(INotification notification, Func<INotification, ValueTask> onSuccess, Func<INotification, ValueTask> onFailure)
        {
            _onSuccess = onSuccess;
            _onFailure = onFailure;
            Notification = notification;
        }

        public ValueTask Commit()
        {
            _committed = true;
            return _onSuccess.Invoke(Notification);
        }

        public ValueTask DisposeAsync()
        {
            if (_committed)
            {
                return ValueTask.CompletedTask;
            }

            return _onFailure.Invoke(Notification);
        }
    }
}
