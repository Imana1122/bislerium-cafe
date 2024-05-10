using BlazorBootstrap;
using Microsoft.AspNetCore.SignalR;

namespace WebUI.Hubs
{
    public class CommunicationHub: Hub
    {
        public async Task Notification(string userId, string message)
        {
            await Clients.All.SendAsync("Notification", userId, message);
        }

    }
}
