using ChatApp.Application.DTOs;

namespace ChatApp.Application.Interfaces.Services;

public interface ISignalRNotifier
{
    Task BroadcastMessageAsync(string roomName, MessageDto message);
}