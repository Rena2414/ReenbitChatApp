using ChatApp.Application.DTOs;
using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.CreateRoom;

public record CreateRoomCommand(string Name, Guid UserId) : IRequest<ChatRoomDto>;