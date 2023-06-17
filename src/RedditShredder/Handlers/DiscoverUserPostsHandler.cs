using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;
using RedditShredder.Notifications;

namespace RedditShredder.Handlers
{
    public class DiscoverUserPostsHandler : INotificationHandler<UserDiscovered>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly WorkQueue _queue;

        public DiscoverUserPostsHandler(WorkQueue queue)
        {
            _queue = queue;
        }

        public async Task Handle(UserDiscovered notification, CancellationToken cancellationToken)
        {
            var pageIndex = 0;
            var lastPost = "";
            var seenPosts = new HashSet<string>();

            while (!cancellationToken.IsCancellationRequested)
            {
                pageIndex++;

                Logger.Info($"Reading posts page '{pageIndex}' for user '{notification.User.Id}'.");
                var posts = notification.User.GetPostHistory(context: 1, limit: 25, sort: "new", after: lastPost)
                                        .Where(x => !seenPosts.Contains(x.Id))
                                        .ToList();

                foreach (var unseenPosts in posts)
                {
                    Logger.Debug($"Discovered post '{unseenPosts.Id}'.");
                    seenPosts.Add(unseenPosts.Id);
                    await _queue.Push(new PostDiscovered(unseenPosts), cancellationToken);
                }

                if (posts.LastOrDefault()?.Fullname is { } last)
                {
                    lastPost = last;
                }
                else
                {
                    Logger.Info($"Discovered '{seenPosts.Count}' posts.");
                    return;
                }
            }
        }
    }
}
