using MediatR;
using Reddit.Controllers;

namespace RedditShredder.Notifications
{
    public record UserDiscovered(User User) : INotification;
}
