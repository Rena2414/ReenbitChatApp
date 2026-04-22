using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.AddUser;

// We need the RequestingUserId to ensure only members of a room can invite others
public record AddUserToRoomCommand(Guid RoomId, string Username, Guid RequestingUserId) : IRequest;