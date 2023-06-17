using MediatR;
using Reddit.Controllers;

namespace RedditShredder.Notifications
{
    public record PostDiscovered(Post Post) : INotification;
}