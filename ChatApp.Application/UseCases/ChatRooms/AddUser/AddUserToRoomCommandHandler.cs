using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities; // Needed for nameof(ChatRoom) and nameof(User)
using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.AddUser;

public class AddUserToRoomCommandHandler : IRequestHandler<AddUserToRoomCommand>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IUserRepository _userRepository;

    public AddUserToRoomCommandHandler(IChatRoomRepository chatRoomRepository, IUserRepository userRepository)
    {
        _chatRoomRepository = chatRoomRepository;
        _userRepository = userRepository;
    }

    public async Task Handle(AddUserToRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _chatRoomRepository.GetByIdAsync(request.RoomId, cancellationToken)
                   // FIX: Pass entity name and the ID we searched for
                   ?? throw new NotFoundException(nameof(ChatRoom), request.RoomId);

        // Security check: only users already IN the room can add new users
        if (!room.Users.Any(u => u.Id == request.RequestingUserId))
        {
            throw new UnauthorizedException("You are not a member of this chat room.");
        }

        var userToAdd = await _userRepository.GetByUsernameAsync(request.Username, cancellationToken)
                        // FIX: Pass entity name and the username we searched for
                        ?? throw new NotFoundException(nameof(User), request.Username);

        // Only add them if they aren't already in the room
        if (!room.Users.Any(u => u.Id == userToAdd.Id))
        {
            await _chatRoomRepository.AddUserToRoomAsync(request.RoomId, userToAdd.Id, cancellationToken);
        }
    }
}