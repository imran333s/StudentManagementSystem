using StudentManagement.Core.Entities;

namespace StudentManagement.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User> AddAsync(User user);
    Task<bool> ExistsAsync(string username);
}
