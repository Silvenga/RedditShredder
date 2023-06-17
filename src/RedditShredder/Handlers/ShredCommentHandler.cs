using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;
using RedditShredder.Generation;
using RedditShredder.Notifications;

namespace RedditShredder.Handlers
{
    public class ShredCommentHandler : INotificationHandler<CommentDiscovered>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ContentGenerator _generator;

        public ShredCommentHandler(ContentGenerator generator)
        {
            _generator = generator;
        }

        public async Task Handle(CommentDiscovered notification, CancellationToken cancellationToken)
        {
            var comment = notification.Comment;

            var targetContent = _generator.GenerateCommentForId(comment.Id);

            if (comment.Body != targetContent)
            {
                await comment.EditAsync(targetContent);
                Logger.Info($"Updated comment '{comment.Id}', url: https://reddit.com{comment.Permalink}.");
            }
        }
    }
}
