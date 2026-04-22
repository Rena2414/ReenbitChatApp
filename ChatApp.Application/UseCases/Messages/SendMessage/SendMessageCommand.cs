using ChatApp.Application.DTOs;
using ChatApp.Application.Exceptions;
using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Application.Interfaces.Services;
using ChatApp.Domain.Entities;
using MediatR;

namespace ChatApp.Application.UseCases.Messages.SendMessage;

public record SendMessageCommand(string Content, Guid ChatRoomId, Guid UserId, string Username, string RoomName) : IRequest<MessageDto>;

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
        if (string.IsNullOrWhiteSpace(request.Content))
            throw new ValidationException("Message content cannot be empty.");

        if (request.Content.Length > 500)
            throw new ValidationException("Message cannot exceed 500 characters.");

        var sentiment = await _sentimentService.AnalyzeTextAsync(request.Content, cancellationToken);

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            ChatRoomId = request.ChatRoomId,
            UserId = request.UserId,
            Sentiment = sentiment,
            Timestamp = DateTime.UtcNow
        };

        await _messageRepository.AddMessageAsync(message, cancellationToken);
        await _messageRepository.SaveChangesAsync(cancellationToken);

        var messageDto = new MessageDto(
            message.Id, 
            message.Content, 
            request.Username, 
            message.Timestamp, 
            message.Sentiment.ToString(),
            message.UserId
        );

        await _signalRNotifier.BroadcastMessageAsync(request.RoomName, messageDto);

        return messageDto;
    }
}