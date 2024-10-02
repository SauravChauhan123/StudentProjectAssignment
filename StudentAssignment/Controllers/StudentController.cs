using Application.DTOs;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace StudentAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        #region Dependencies

        private readonly IStudentRepository _studentRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IMapper _mapper;
        private readonly StudentHelper _studentHelper;

        #endregion

        #region Constructor

        public StudentController(IStudentRepository studentRepo, IMapper mapper, IProjectRepository projectRepo)
        {
            _studentRepo = studentRepo ?? throw new ArgumentNullException(nameof(studentRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _projectRepo = projectRepo ?? throw new ArgumentNullException(nameof(projectRepo));
            _studentHelper = new StudentHelper(projectRepo);
        }

        #endregion

        #region Student Management

        /// <summary>
        /// Retrieves a student by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <returns>Returns the student details if found; otherwise, a NotFound result.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetStudent(Guid id)
        {
            try
            {
                var student = await _studentRepo.GetByIdAsync(id);
                if (student == null)
                {
                    return NotFound();
                }

                var studentDto = _mapper.Map<StudentDtos>(student);
                return Ok(studentDto);
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while retrieving the student.");
            }
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="studentDto">The student data transfer object.</param>
        /// <returns>Returns the created student details.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDto studentDto)
        {
            if (studentDto == null)
            {
                return BadRequest("Student data is required.");
            }

            try
            {
                // Fetch or create associated projects
                var projects = await _studentHelper.FetchOrCreateProjectsAsync(studentDto.ProjectNames);
                var student = _mapper.Map<Student>(studentDto);
                student.Projects = projects.ToList();

                // Save the student in the database
                var createdStudent = await _studentRepo.AddAsync(student);
                return CreatedAtAction(nameof(GetStudent), new { id = createdStudent.StudentId }, createdStudent);
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while creating the student.");
            }
        }

        /// <summary>
        /// Updates an existing student.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <param name="studentDto">The student data transfer object.</param>
        /// <returns>NoContent if successful; otherwise, a NotFound or BadRequest result.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateStudent(Guid id, [FromBody] StudentDto studentDto)
        {
            if (studentDto == null || id != studentDto.StudentId)
            {
                return BadRequest("Invalid student data.");
            }

            try
            {
                // Create a new Student instance
                var student = new Student
                {
                    StudentId = studentDto.StudentId,
                    Name = studentDto.Name,
                    Projects = studentDto.ProjectNames.Select(name => new Project { Name = name }).ToList() // Convert project names to Project objects
                };
                await _studentRepo.UpdateAsync(student);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while updating the student.");
            }
        }

        /// <summary>
        /// Deletes a student by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <returns>NoContent if successful; otherwise, a NotFound result.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteStudent(Guid id)
        {
            try
            {
                var studentExists = await _studentRepo.GetByIdAsync(id);
                if (studentExists == null)
                {
                    return NotFound();
                }

                await _studentRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while deleting the student.");
            }
        }

        /// <summary>
        /// Retrieves the count of projects associated with a specific student.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <returns>Returns the count of projects associated with the student.</returns>
        [HttpGet("{studentId:guid}/projects-count")]
        public async Task<IActionResult> GetProjectsCountForStudent(Guid studentId)
        {
            try
            {
                var count = await _studentRepo.GetProjectsCountByStudentIdAsync(studentId);
                return Ok(count);
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while retrieving the project count for the student.");
            }
        }

        /// <summary>
        /// Retrieves all students with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of students to retrieve per page.</param>
        /// <returns>Returns a paginated list of students.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var students = await _studentRepo.GetAllAsync(pageNumber, pageSize);
                var studentDtos = _mapper.Map<IEnumerable<StudentDtos>>(students);

                return Ok(new
                {
                    TotalCount = students.Count(),
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Students = studentDtos
                });
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while retrieving the students.");
            }
        }

        #endregion
    }
}
