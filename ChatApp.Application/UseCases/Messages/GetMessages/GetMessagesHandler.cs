using ChatApp.Application.DTOs;
using ChatApp.Application.Interfaces.Repositories;
using MediatR;

namespace ChatApp.Application.UseCases.Messages.GetMessages;

public class GetMessagesHandler : IRequestHandler<GetMessagesQuery, IEnumerable<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessagesHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<IEnumerable<MessageDto>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetMessagesByRoomAsync(request.ChatRoomId, cancellationToken);
        
        return messages.Select(m => new MessageDto(
            m.Id,
            m.Content,
            m.User?.Username ?? "Unknown",
            m.Timestamp,
            m.Sentiment.ToString(),
            m.UserId
        ));
    }
}