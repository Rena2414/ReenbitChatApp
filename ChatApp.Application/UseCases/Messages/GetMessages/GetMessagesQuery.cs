using ChatApp.Application.DTOs;
using MediatR;

namespace ChatApp.Application.UseCases.Messages.GetMessages;

public record GetMessagesQuery(Guid ChatRoomId) : IRequest<IEnumerable<MessageDto>>;