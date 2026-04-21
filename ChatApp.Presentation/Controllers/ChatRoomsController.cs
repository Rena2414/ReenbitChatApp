using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatRoomsController : ControllerBase
{
    private readonly IChatRoomRepository _chatRoomRepository;

    public ChatRoomsController(IChatRoomRepository chatRoomRepository)
    {
        _chatRoomRepository = chatRoomRepository;
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
        return Ok(room);
    }
}