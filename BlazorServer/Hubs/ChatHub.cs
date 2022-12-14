using Microsoft.AspNetCore.SignalR; //wrapper around websockets


namespace BlazorServer.Hubs;

public class ChatHub: Hub
{
    public Task SendMessage(string user, string message)
    {
        
        return Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}
