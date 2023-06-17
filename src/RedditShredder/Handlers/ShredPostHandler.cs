using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NLog;
using Reddit.Controllers;
using Reddit.Exceptions;
using RedditShredder.Generation;
using RedditShredder.Notifications;

namespace RedditShredder.Handlers
{
    public class ShredPostHandler : INotificationHandler<PostDiscovered>
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ContentGenerator _generator;

        public ShredPostHandler(ContentGenerator generator)
        {
            _generator = generator;
        }

        public async Task Handle(PostDiscovered notification, CancellationToken cancellationToken)
        {
            var post = notification.Post;

            if (post is SelfPost selfPost)
            {
                var targetContent = _generator.GeneratePostForId(post.Id);

                if (selfPost.SelfText != targetContent)
                {
                    try
                    {
                        await selfPost.EditAsync(targetContent);
                        Logger.Info($"Updated post '{post.Id}', url: https://reddit.com{post.Permalink}.");
                    }
                    catch (RedditControllerException e) when (e.Message.Contains(" somewhere in your post body"))
                    {
                        Logger.Warn($"Failed to update the post 'https://reddit.com{post.Permalink}', this is likely an auto-mod restriction.");
                    }
                }
            }
        }
    }
}
