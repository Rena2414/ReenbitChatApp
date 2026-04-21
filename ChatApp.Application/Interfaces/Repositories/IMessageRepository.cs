using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IMessageRepository
{
    Task AddMessageAsync(Message message, CancellationToken cancellationToken);
    Task<IEnumerable<Message>> GetMessagesByRoomAsync(Guid roomId, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}