using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Retrieves a project by its unique identifier, including associated students.
        public async Task<Project> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid project ID.", nameof(id));
            }

            try
            {
                return await _context.Projects
                    .Include(p => p.Students)
                    .FirstOrDefaultAsync(p => p.ProjectId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the project.", ex);
            }
        }

        // Retrieves all projects with pagination support and includes associated students.
        public async Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page number and page size must be greater than zero.");
            }

            try
            {
                return await _context.Projects
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(p => p.Students)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the projects.", ex);
            }
        }

        // Adds a new project to the database and saves changes.
        public async Task<Project> AddAsync(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project), "Project cannot be null.");
            }

            try
            {
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();
                return project;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the project.", ex);
            }
        }

        // Updates an existing project in the database.
        public async Task UpdateProjectAsync(Project project)
        {
            if (project == null)
            {
                throw new ArgumentNullException(nameof(project), "Project cannot be null.");
            }

            try
            {
                _context.Entry(project).State = EntityState.Modified;
                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("The project you attempted to update does not exist.");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the project.", ex);
            }
        }

        // Deletes a project by its unique identifier if it exists.
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid project ID.", nameof(id));
            }

            try
            {
                var project = await _context.Projects.FindAsync(id);
                if (project != null)
                {
                    _context.Projects.Remove(project);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the project.", ex);
            }
        }

        // Retrieves students associated with a specific project.
        public async Task<IEnumerable<Student>> GetStudentsByProjectAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("Invalid project ID.", nameof(projectId));
            }

            try
            {
                var project = await GetByIdAsync(projectId);
                return project?.Students ?? new List<Student>();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving students for the project.", ex);
            }
        }

        // Retrieves projects based on a list of project names.
        public async Task<IEnumerable<Project>> GetProjectByNamesAsync(IEnumerable<string> projectNames)
        {
            if (projectNames == null || !projectNames.Any())
            {
                throw new ArgumentException("Project names cannot be null or empty.", nameof(projectNames));
            }

            try
            {
                return await _context.Projects
                    .Where(s => projectNames.Contains(s.Name))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving projects by names.", ex);
            }
        }
    }
}


