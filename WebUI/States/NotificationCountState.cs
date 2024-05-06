using Application.Service.Notifications;
using MediatR;

namespace WebUI.States
{
    public class NotificationCountState(IServiceProvider serviceProvider)
    {
        public int notificationsCount { get; set; }
        
        public event Action StateChanged;
        public async Task GetActiveOrdersCount(Guid userId)
        {
            using var scope = serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var response = (await mediator.Send(new GetUnreadUserNotificationCountQuery(userId)));
            notificationsCount = response;

            if (StateChanged != null)
            {
                StateChanged.Invoke();
            }
        }
    }
}
