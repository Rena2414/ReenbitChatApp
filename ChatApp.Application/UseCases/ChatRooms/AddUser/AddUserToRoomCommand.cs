using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.AddUser;

public record AddUserToRoomCommand(Guid RoomId, string Username, Guid RequestingUserId) : IRequest;