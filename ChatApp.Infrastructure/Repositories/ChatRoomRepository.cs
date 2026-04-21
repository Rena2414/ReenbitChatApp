using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class ChatRoomRepository : IChatRoomRepository
{
    private readonly ChatAppContext _context;

    public ChatRoomRepository(ChatAppContext context)
    {
        _context = context;
    }

    public async Task<ChatRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.ChatRooms.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<ChatRoom>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.ChatRooms
            .AsNoTracking()
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ChatRoom chatRoom, CancellationToken cancellationToken)
    {
        await _context.ChatRooms.AddAsync(chatRoom, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}