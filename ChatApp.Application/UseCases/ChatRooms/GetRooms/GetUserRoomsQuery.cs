using ChatApp.Application.DTOs;
using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.GetRooms;

public record GetUserRoomsQuery(Guid UserId) : IRequest<IEnumerable<ChatRoomDto>>;