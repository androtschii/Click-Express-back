using Microsoft.AspNetCore.SignalR;

namespace ClickExpress.Api.Hubs
{
    public class OrderHub : Hub
    {
        public async Task SubscribeToOrder(int orderId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"order-{orderId}");
        }

        public async Task UnsubscribeFromOrder(int orderId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"order-{orderId}");
        }
    }
}
