using System.Security.Claims;
using ChatApp.Application.UseCases.ChatRooms.AddUser;
using ChatApp.Application.UseCases.ChatRooms.CreateRoom;
using ChatApp.Application.UseCases.ChatRooms.GetRooms;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatRoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatRoomsController(IMediator mediator) => _mediator = mediator;

    private Guid GetCurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetUserRooms(CancellationToken cancellationToken)
    {
        var query = new GetUserRoomsQuery(GetCurrentUserId());
        return Ok(await _mediator.Send(query, cancellationToken));
    }
    
    [HttpPost("{roomId:guid}/users")]
    public async Task<IActionResult> AddUserToRoom(Guid roomId, [FromBody] string usernameToAdd, CancellationToken cancellationToken)
    {
        var command = new AddUserToRoomCommand(roomId, usernameToAdd, GetCurrentUserId());
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    // NOTE: Aligns with the runtime validation error expecting "roomName"
    public record CreateRoomRequest(string RoomName, List<Guid> ParticipantIds);

    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRoomCommand(request.RoomName, GetCurrentUserId(), request.ParticipantIds);
        var room = await _mediator.Send(command, cancellationToken);
        return Ok(room);
    }
}