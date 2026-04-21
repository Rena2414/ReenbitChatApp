using ChatApp.Application.Interfaces.Repositories;
using ChatApp.Domain.Entities;
using ChatApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatAppContext _context;

    public MessageRepository(ChatAppContext context)
    {
        _context = context;
    }

    public async Task AddMessageAsync(Message message, CancellationToken cancellationToken)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetMessagesByRoomAsync(Guid roomId, CancellationToken cancellationToken)
    {
        return await _context.Messages
            .AsNoTracking()
            .Include(m => m.User)
            .Where(m => m.ChatRoomId == roomId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}