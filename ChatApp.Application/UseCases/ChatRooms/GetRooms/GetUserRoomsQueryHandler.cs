using ChatApp.Application.DTOs;
using ChatApp.Application.Interfaces.Repositories;
using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.GetRooms;

public class GetUserRoomsQueryHandler : IRequestHandler<GetUserRoomsQuery, IEnumerable<ChatRoomDto>>
{
    private readonly IChatRoomRepository _chatRoomRepository;

    public GetUserRoomsQueryHandler(IChatRoomRepository chatRoomRepository)
    {
        _chatRoomRepository = chatRoomRepository;
    }

    public async Task<IEnumerable<ChatRoomDto>> Handle(GetUserRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _chatRoomRepository.GetRoomsForUserAsync(request.UserId, cancellationToken);
        
        return rooms.Select(r => new ChatRoomDto(
            r.Id, 
            r.Name, 
            r.CreatedAt,
            r.Users.Select(u => new UserDto(u.Id, u.Username)).ToList()
        ));
    }
}