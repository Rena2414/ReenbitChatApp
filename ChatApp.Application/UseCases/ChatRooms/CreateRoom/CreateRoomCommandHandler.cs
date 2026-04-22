using ChatApp.Application.DTOs;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.CreateRoom;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ChatRoomDto>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISignalRNotifier _signalRNotifier;

    public CreateRoomCommandHandler(
        IChatRoomRepository chatRoomRepository, 
        IUserRepository userRepository,
        ISignalRNotifier signalRNotifier)
    {
        _chatRoomRepository = chatRoomRepository;
        _userRepository = userRepository;
        _signalRNotifier = signalRNotifier;
    }

    public async Task<ChatRoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ValidationException("Room name is required.");

        // 1. Initialize the room
        var room = new ChatRoom 
        { 
            Id = Guid.NewGuid(), 
            Name = request.Name.Trim(),
            CreatedAt = DateTime.UtcNow,
            Users = new List<User>()
        };

        // 2. Combine Creator + Selected Participants
        var allUserIds = request.ParticipantIds.Append(request.CreatorId).Distinct();

        foreach (var userId in allUserIds)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
                       ?? throw new NotFoundException(nameof(User), userId);
            room.Users.Add(user);
        }

        // 3. Persist
        await _chatRoomRepository.AddAsync(room, cancellationToken);
        await _chatRoomRepository.SaveChangesAsync(cancellationToken);
        
        // 4. Map to DTO including participants
        var participantDtos = room.Users.Select(u => new UserDto(u.Id, u.Username)).ToList();
        var roomDto = new ChatRoomDto(room.Id, room.Name, room.CreatedAt, participantDtos);

        // 5. Broadcast to all involved users
        await _signalRNotifier.BroadcastRoomCreatedAsync(roomDto);
        
        return roomDto;
    }
}