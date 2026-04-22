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
        await _hubContext.Clients.Group(roomName).SendAsync("ReceiveMessage", message);
    }

    public async Task BroadcastRoomCreatedAsync(ChatRoomDto room)
    {
        await _hubContext.Clients.All.SendAsync("RoomCreated", room);
    }
}