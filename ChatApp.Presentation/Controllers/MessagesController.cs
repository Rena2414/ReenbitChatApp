using ChatApp.Application.DTOs;
using ChatApp.Application.UseCases.Messages.GetMessages;
using ChatApp.Application.UseCases.Messages.SendMessage;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{roomId}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(Guid roomId)
    {
        var result = await _mediator.Send(new GetMessagesQuery(roomId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}