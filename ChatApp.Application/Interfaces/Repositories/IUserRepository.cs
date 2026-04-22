using ChatApp.Domain.Entities;

namespace ChatApp.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}