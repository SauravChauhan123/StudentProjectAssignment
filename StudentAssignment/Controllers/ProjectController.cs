using Application.DTOs;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace StudentAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        #region Dependencies

        private readonly IProjectRepository _projectRepo;
        private readonly IMapper _mapper;
        private readonly ProjectHelper _projectHelper;
        private readonly IStudentRepository _studentRepository;

        #endregion

        #region Constructor

        public ProjectController(IProjectRepository projectRepo, IMapper mapper, IStudentRepository studentRepository)
        {
            _projectRepo = projectRepo ?? throw new ArgumentNullException(nameof(projectRepo));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _projectHelper = new ProjectHelper(studentRepository ?? throw new ArgumentNullException(nameof(studentRepository)));
            _studentRepository = studentRepository;
        }

        #endregion

        #region Project Management

        /// <summary>
        /// Retrieves a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <returns>Returns the project details if found; otherwise, a NotFound result.</returns>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            try
            {
                var project = await _projectRepo.GetByIdAsync(id);
                if (project == null)
                {
                    return NotFound();
                }

                var projectDto = _mapper.Map<ProjectDto>(project);
                return Ok(projectDto);
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while retrieving the project.");
            }
        }

        /// <summary>
        /// Creates a new project.
        /// </summary>
        /// <param name="projectDto">The project data transfer object.</param>
        /// <returns>Returns the created project details.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectDto projectDto)
        {
            if (projectDto == null)
            {
                return BadRequest("Project data is required.");
            }

            try
            {
                // Use the helper method to add new students
                await _projectHelper.AddNewStudentsIfNeededAsync(projectDto.StudentNames);

                // Map the ProjectDto to the Project entity
                var project = _mapper.Map<Project>(projectDto);

                // Re-fetch existing students after adding new ones
                var students = await _studentRepository.GetStudentsByNamesAsync(projectDto.StudentNames);
                project.Students = students.ToList();

                // Save the project in the database
                var createdProject = await _projectRepo.AddAsync(project);

                // Return 201 Created with location of the created resource
                return CreatedAtAction(nameof(GetProjectById), new { id = createdProject.ProjectId }, createdProject);
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while creating the project.");
            }
        }

        /// <summary>
        /// Deletes a project by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the project.</param>
        /// <returns>NoContent if successful; otherwise, a NotFound result.</returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            try
            {
                var projectExists = await _projectRepo.GetByIdAsync(id);
                if (projectExists == null)
                {
                    return NotFound();
                }

                await _projectRepo.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while deleting the project.");
            }
        }

        /// <summary>
        /// Retrieves all students associated with a specific project.
        /// </summary>
        /// <param name="projectId">The unique identifier of the project.</param>
        /// <returns>Returns the list of students associated with the project.</returns>
        [HttpGet("{projectId:guid}/students")]
        public async Task<IActionResult> GetStudentsByProject(Guid projectId)
        {
            try
            {
                var students = await _projectRepo.GetStudentsByProjectAsync(projectId);
                if (students == null || !students.Any())
                {
                    return NotFound();
                }

                var studentDtos = _mapper.Map<IEnumerable<StudentDtos>>(students);
                return Ok(studentDtos);
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while retrieving the students.");
            }
        }

        /// <summary>
        /// Updates an existing project identified by its unique ID.
        /// </summary>
        /// <param name="id">The unique identifier of the project to update.</param>
        /// <param name="projectDto">The DTO containing the updated project information.</param>
        /// <returns>An IActionResult indicating the result of the update operation.</returns>
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectDto projectDto)
        {
            // Validate the input DTO and ensure the ID matches the DTO's ProjectId
            if (projectDto == null || id != projectDto.ProjectId)
            {
                return BadRequest(); // Return a BadRequest response if validation fails
            }

            // Create a new Project instance using the values from the DTO
            var project = new Project
            {
                ProjectId = projectDto.ProjectId,
                Name = projectDto.Name,
                // Convert student names from the DTO to Student objects
                Students = projectDto.StudentNames.Select(name => new Student { Name = name }).ToList() // Create a list of Student objects based on names
            };

            // Update the project in the repository
            await _projectRepo.UpdateProjectAsync(project);

            return NoContent(); // Return a NoContent response to indicate the update was successful
        }
        /// <summary>
        /// Retrieves all projects with pagination support.
        /// </summary>
        /// <param name="pageNumber">The page number for pagination.</param>
        /// <param name="pageSize">The number of projects to retrieve per page.</param>
        /// <returns>Returns a paginated list of projects.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllProjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var projects = await _projectRepo.GetAllAsync(pageNumber, pageSize);
                var projectDtos = _mapper.Map<IEnumerable<ProjectDto>>(projects);

                return Ok(new
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Projects = projectDtos
                });
            }
            catch (Exception ex)
            {
                // Log exception (implementation depends on your logging setup)
                return StatusCode(500, "An error occurred while retrieving the projects.");
            }
        }

        #endregion
    }
}
