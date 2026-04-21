using ChatApp.Application.DTOs;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.Features.Messages;

// The Command: What data is needed to perform the action?
public record SendMessageCommand(string Content, Guid ChatRoomId, Guid UserId, string Username, string RoomName) : IRequest<MessageDto>;

// The Handler: The business logic executed when the command is sent.
public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly ISentimentService _sentimentService;
    private readonly ISignalRNotifier _signalRNotifier;

    public SendMessageCommandHandler(
        IMessageRepository messageRepository, 
        ISentimentService sentimentService, 
        ISignalRNotifier signalRNotifier)
    {
        _messageRepository = messageRepository;
        _sentimentService = sentimentService;
        _signalRNotifier = signalRNotifier;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        // 1. Optional Sentiment Analysis (Non-blocking ideally, but awaited here for simplicity)
        var sentiment = await _sentimentService.AnalyzeTextAsync(request.Content, cancellationToken);

        // 2. Create the Domain Entity
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            ChatRoomId = request.ChatRoomId,
            UserId = request.UserId,
            Sentiment = sentiment,
            Timestamp = DateTime.UtcNow
        };

        // 3. Save to Database
        await _messageRepository.AddMessageAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        // 4. Map to DTO
        var messageDto = new MessageDto(
            message.Id, 
            message.Content, 
            request.Username, 
            message.Timestamp, 
            message.Sentiment.ToString());

        // 5. Broadcast via SignalR
        await _signalRNotifier.BroadcastMessageAsync(request.RoomName, messageDto);

        return messageDto;
    }
}