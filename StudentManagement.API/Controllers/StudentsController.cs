using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagement.Application.DTOs;
using StudentManagement.Application.Interfaces.Services;

namespace StudentManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Retrieves all students.
    /// </summary>
    /// <returns>A list of students.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<StudentDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<StudentDto>>>> GetAll()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(ApiResponse<IEnumerable<StudentDto>>.SuccessResponse(students, "Students retrieved successfully."));
    }

    /// <summary>
    /// Retrieves a specific student by unique identifier.
    /// </summary>
    /// <param name="id">The student identifier.</param>
    /// <returns>The requested student details.</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        return Ok(ApiResponse<StudentDto>.SuccessResponse(student, "Student retrieved successfully."));
    }

    /// <summary>
    /// Creates a new student record.
    /// </summary>
    /// <param name="createStudentDto">The student input data.</param>
    /// <returns>The created student details.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<StudentDto>>> Create([FromBody] CreateStudentDto createStudentDto)
    {
        var createdStudent = await _studentService.CreateStudentAsync(createStudentDto);
        var response = ApiResponse<StudentDto>.SuccessResponse(createdStudent, "Student created successfully.");
        return CreatedAtAction(nameof(GetById), new { id = createdStudent.Id }, response);
    }

    /// <summary>
    /// Updates an existing student record.
    /// </summary>
    /// <param name="id">The student identifier to update.</param>
    /// <param name="updateStudentDto">The updated student data.</param>
    /// <returns>A success acknowledgment response.</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateStudentDto updateStudentDto)
    {
        await _studentService.UpdateStudentAsync(id, updateStudentDto);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Student updated successfully."));
    }

    /// <summary>
    /// Deletes a specific student record.
    /// </summary>
    /// <param name="id">The student identifier to delete.</param>
    /// <returns>A success acknowledgment response.</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        await _studentService.DeleteStudentAsync(id);
        return Ok(ApiResponse<object>.SuccessResponse(null, "Student deleted successfully."));
    }
}
