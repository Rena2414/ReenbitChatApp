using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatRoomsController : ControllerBase
{
    private readonly IChatRoomRepository _chatRoomRepository;
    private readonly ISignalRNotifier _signalRNotifier; // Add notifier field

    // Inject the notifier
    public ChatRoomsController(IChatRoomRepository chatRoomRepository, ISignalRNotifier signalRNotifier)
    {
        _chatRoomRepository = chatRoomRepository;
        _signalRNotifier = signalRNotifier;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatRoom>>> GetRooms(CancellationToken cancellationToken)
    {
        var rooms = await _chatRoomRepository.GetAllAsync(cancellationToken);
        return Ok(rooms);
    }

    [HttpPost]
    public async Task<ActionResult<ChatRoom>> CreateRoom([FromBody] string roomName, CancellationToken cancellationToken)
    {
        var room = new ChatRoom { Id = Guid.NewGuid(), Name = roomName };
        await _chatRoomRepository.AddAsync(room, cancellationToken);
        await _chatRoomRepository.SaveChangesAsync(cancellationToken);
        
        // Broadcast the new room to all users
        await _signalRNotifier.BroadcastRoomCreatedAsync(room);

        return Ok(room);
    }
}