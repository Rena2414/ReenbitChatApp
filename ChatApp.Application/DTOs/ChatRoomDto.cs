namespace ChatApp.Application.DTOs;

public record ChatRoomDto(
    Guid Id, 
    string Name, 
    DateTime CreatedAt, 
    IEnumerable<UserDto> Participants);