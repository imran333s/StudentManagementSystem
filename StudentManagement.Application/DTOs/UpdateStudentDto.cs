using System.ComponentModel.DataAnnotations;

namespace StudentManagement.Application.DTOs;

public class UpdateStudentDto
{
    [Required(ErrorMessage = "Student Id is required.")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
    public string Email { get; set; } = string.Empty;

    [Range(5, 100, ErrorMessage = "Age must be between 5 and 100.")]
    public int Age { get; set; }

    [Required(ErrorMessage = "Course is required.")]
    [StringLength(100, ErrorMessage = "Course cannot exceed 100 characters.")]
    public string Course { get; set; } = string.Empty;
}
