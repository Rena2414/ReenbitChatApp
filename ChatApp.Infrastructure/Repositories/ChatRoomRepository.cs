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
        // IMPORTANT: .Include(c => c.Users) is vital for your Application handlers
        // so that EF Core loads the participants when checking permissions!
        return await _context.ChatRooms
            .Include(c => c.Users)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ChatRoom>> GetRoomsForUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        // Fetches only the rooms where this specific user is a participant
        return await _context.ChatRooms
            .Where(r => r.Users.Any(u => u.Id == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ChatRoom chatRoom, CancellationToken cancellationToken)
    {
        await _context.ChatRooms.AddAsync(chatRoom, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddUserToRoomAsync(Guid roomId, Guid userId, CancellationToken cancellationToken)
    {
        var room = await _context.ChatRooms
            .Include(r => r.Users)
            .FirstOrDefaultAsync(r => r.Id == roomId, cancellationToken);
            
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (room != null && user != null)
        {
            room.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
    
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}