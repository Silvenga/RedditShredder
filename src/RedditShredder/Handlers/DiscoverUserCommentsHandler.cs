using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;
using RedditShredder.Notifications;

namespace RedditShredder.Handlers
{
    public class DiscoverUserCommentsHandler : INotificationHandler<UserDiscovered>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly WorkQueue _queue;

        public DiscoverUserCommentsHandler(WorkQueue queue)
        {
            _queue = queue;
        }

        public async Task Handle(UserDiscovered notification, CancellationToken cancellationToken)
        {
            var pageIndex = 0;
            var lastComment = "";
            var seenComments = new HashSet<string>();

            while (!cancellationToken.IsCancellationRequested)
            {
                pageIndex++;

                Logger.Info($"Reading comment page '{pageIndex}' for user '{notification.User.Id}'.");
                var comments = notification.User.GetCommentHistory(context: 1, limit: 25, sort: "new", after: lastComment)
                                           .Where(x => !seenComments.Contains(x.Id))
                                           .ToList();

                foreach (var unseenPosts in comments)
                {
                    Logger.Debug($"Discovered comment '{unseenPosts.Id}'.");
                    seenComments.Add(unseenPosts.Id);
                    await _queue.Push(new CommentDiscovered(unseenPosts), cancellationToken);
                }

                if (comments.LastOrDefault()?.Fullname is { } last)
                {
                    lastComment = last;
                }
                else
                {
                    Logger.Info($"Discovered '{seenComments.Count}' comments.");
                    return;
                }
            }
        }
    }
}
