using StudentManagement.Application.DTOs;
using StudentManagement.Application.Exceptions;
using StudentManagement.Application.Interfaces.Services;
using StudentManagement.Core.Entities;
using StudentManagement.Core.Interfaces.Repositories;

namespace StudentManagement.Application.Services;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Select(s => new StudentDto
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Age = s.Age,
            Course = s.Course,
            CreatedDate = s.CreatedDate
        });
    }

    public async Task<StudentDto> GetStudentByIdAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
        {
            throw new NotFoundException(nameof(Student), id);
        }

        return new StudentDto
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Age = student.Age,
            Course = student.Course,
            CreatedDate = student.CreatedDate
        };
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
    {
        var existingStudent = await _studentRepository.GetByEmailAsync(createStudentDto.Email);
        if (existingStudent != null)
        {
            throw new BadRequestException($"A student with email '{createStudentDto.Email}' already exists.");
        }

        var student = new Student
        {
            Name = createStudentDto.Name,
            Email = createStudentDto.Email,
            Age = createStudentDto.Age,
            Course = createStudentDto.Course,
            CreatedDate = DateTime.UtcNow
        };

        var createdStudent = await _studentRepository.AddAsync(student);

        return new StudentDto
        {
            Id = createdStudent.Id,
            Name = createdStudent.Name,
            Email = createdStudent.Email,
            Age = createdStudent.Age,
            Course = createdStudent.Course,
            CreatedDate = createdStudent.CreatedDate
        };
    }

    public async Task UpdateStudentAsync(int id, UpdateStudentDto updateStudentDto)
    {
        if (id != updateStudentDto.Id)
        {
            throw new BadRequestException("Student ID in the route does not match the ID in the request body.");
        }

        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
        {
            throw new NotFoundException(nameof(Student), id);
        }

        var existingWithEmail = await _studentRepository.GetByEmailAsync(updateStudentDto.Email);
        if (existingWithEmail != null && existingWithEmail.Id != id)
        {
            throw new BadRequestException($"A student with email '{updateStudentDto.Email}' already exists.");
        }

        student.Name = updateStudentDto.Name;
        student.Email = updateStudentDto.Email;
        student.Age = updateStudentDto.Age;
        student.Course = updateStudentDto.Course;

        await _studentRepository.UpdateAsync(student);
    }

    public async Task DeleteStudentAsync(int id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        if (student == null)
        {
            throw new NotFoundException(nameof(Student), id);
        }

        await _studentRepository.DeleteAsync(student);
    }
}
