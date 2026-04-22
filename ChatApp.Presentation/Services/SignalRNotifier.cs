using ChatApp.Application.DTOs;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;
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
        await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", message);
    }

    // Add this new method
    public async Task BroadcastRoomCreatedAsync(ChatRoom room)
    {
        // Broadcasts the new room to absolutely everyone connected
        await _hubContext.Clients.All.SendAsync("RoomCreated", room);
    }
}