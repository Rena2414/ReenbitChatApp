namespace ChatApp.Application.DTOs;

public record SendMessageDto(
    string Content, 
    Guid ChatRoomId, 
    Guid UserId, 
    string Username, 
    string RoomName);