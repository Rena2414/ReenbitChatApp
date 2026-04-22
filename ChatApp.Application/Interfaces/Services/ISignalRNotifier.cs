using ChatApp.Application.DTOs;
using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Services;

public interface ISignalRNotifier
{
    Task BroadcastMessageAsync(string roomName, MessageDto message);
    Task BroadcastRoomCreatedAsync(ChatRoom room);
}