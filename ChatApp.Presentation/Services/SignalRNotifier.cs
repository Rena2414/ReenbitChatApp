using ChatApp.Application.DTOs;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Presentation.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Presentation.Services;

public class SignalRNotifier : ISignalRNotifier
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRNotifier(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastMessageAsync(string roomName, MessageDto message)
    {
        // Broadcasts the message to everyone connected to this specific room
        await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", message);
    }
}