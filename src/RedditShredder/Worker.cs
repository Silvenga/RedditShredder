using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;

namespace RedditShredder
{
    public class Worker
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly WorkQueue _workQueue;
        private readonly IMediator _mediator;

        public Worker(WorkQueue workQueue, IMediator mediator)
        {
            _workQueue = workQueue;
            _mediator = mediator;
        }

        public async Task Run(int workerId, CancellationToken cancellationToken = default)
        {
            Logger.Debug($"Starting worker {workerId}.");

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await using var reservation = await _workQueue.Take(cancellationToken);

                    Logger.Trace($"Starting work for {reservation.Notification.GetType()}...");

                    var stopwatch = Stopwatch.StartNew();
                    await _mediator.Publish(reservation.Notification, cancellationToken);
                    await reservation.Commit();

                    Logger.Trace($"Finished work for {reservation.Notification.GetType()} after {stopwatch.ElapsedMilliseconds}ms.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    // Ignored
                }
                catch (Exception e)
                {
                    Logger.Warn(e, "Handled an error during processing.");
                }
            }

            Logger.Debug($"Stopping worker {workerId}.");
        }
    }
}
