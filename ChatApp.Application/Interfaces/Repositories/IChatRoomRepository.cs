using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IChatRoomRepository
{
    Task<ChatRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    
    Task<IEnumerable<ChatRoom>> GetRoomsForUserAsync(Guid userId, CancellationToken cancellationToken);
    
    Task AddAsync(ChatRoom chatRoom, CancellationToken cancellationToken);
    
    Task AddUserToRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken);
    
    Task SaveChangesAsync(CancellationToken cancellationToken);
}