using Microsoft.AspNetCore.SignalR;
namespace dotnet.Hubs
{
    public class OrderHub : Hub
    {
        public async void OrderData(int Status)
        {
            await Clients.All.SendAsync("OrderStatus", Status);
        }

    }
}