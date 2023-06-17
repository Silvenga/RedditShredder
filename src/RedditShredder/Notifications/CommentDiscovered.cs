using MediatR;
using Reddit.Controllers;

namespace RedditShredder.Notifications
{
    public record CommentDiscovered(Comment Comment) : INotification;
}