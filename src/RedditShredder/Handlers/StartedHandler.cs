using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;
using Reddit;
using RedditShredder.Notifications;

namespace RedditShredder.Handlers
{
    public class StartedHandler : INotificationHandler<Started>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly RedditClient _client;
        private readonly WorkQueue _queue;

        public StartedHandler(RedditClient client, WorkQueue queue)
        {
            _client = client;
            _queue = queue;
        }

        public async Task Handle(Started notification, CancellationToken cancellationToken)
        {
            var user = _client.Account.Me;
            Logger.Info($"Discovered user '{user.Id}'.");
            await _queue.Push(new UserDiscovered(user), cancellationToken);
        }
    }
}
