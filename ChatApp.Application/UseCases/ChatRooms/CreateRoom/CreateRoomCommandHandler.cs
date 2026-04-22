using ChatApp.Application.DTOs;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services; // Add this
using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.UseCases.ChatRooms.CreateRoom;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ChatRoomDto>
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly IUserRepository _userRepository;
    private readonly ISignalRNotifier _signalRNotifier; // Add this

    public CreateRoomCommandHandler(
        IChatRoomRepository chatRoomRepository, 
        IUserRepository userRepository,
        ISignalRNotifier signalRNotifier) // Inject it
    {
        _chatRoomRepository = chatRoomRepository;
        _userRepository = userRepository;
        _signalRNotifier = signalRNotifier;
    }

    public async Task<ChatRoomDto> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                   ?? throw new NotFoundException(nameof(User), request.UserId);

        var room = new ChatRoom 
        { 
            Id = Guid.NewGuid(), 
            Name = request.Name 
        };
        
        room.Users.Add(user);
        await _chatRoomRepository.AddAsync(room, cancellationToken);
        await _chatRoomRepository.SaveChangesAsync(cancellationToken); // Ensure saved before broadcast
        
        var roomDto = new ChatRoomDto(room.Id, room.Name, room.CreatedAt);

        // TRIGGER REAL-TIME BROADCAST HERE
        await _signalRNotifier.BroadcastRoomCreatedAsync(roomDto);
        
        return roomDto;
    }
}