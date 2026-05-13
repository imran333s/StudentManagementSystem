using StudentManagement.Core.Entities;

namespace StudentManagement.Core.Interfaces.Repositories;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByEmailAsync(string email);
    Task<Student> AddAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(Student student);
    Task<bool> ExistsAsync(int id);
}
