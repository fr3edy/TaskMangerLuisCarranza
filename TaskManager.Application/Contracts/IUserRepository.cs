using TaskManager.Domain.Entities;

namespace TaskManager.Application.Contracts;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();

    Task AddAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);
}