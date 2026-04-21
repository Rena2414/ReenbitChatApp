using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Presentation.Hubs;

public class ChatHub : Hub
{
    // Clients will call this when they enter a specific chat room UI
    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }
}