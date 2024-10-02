using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Ensure context is not null
        }

        // Retrieves a student by their unique identifier, including associated projects.
        public async Task<Student> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid student ID.", nameof(id)); // Validate student ID
            }

            try
            {
                return await _context.Students.Include(s => s.Projects).FirstOrDefaultAsync(s => s.StudentId == id);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the student.", ex);
            }
        }

        // Retrieves all students with pagination support and includes associated projects.
        public async Task<IEnumerable<Student>> GetAllAsync(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page number and page size must be greater than zero."); // Validate pagination parameters
            }

            try
            {
                return await _context.Students
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Include(s => s.Projects)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving students.", ex);
            }
        }

        // Adds a new student to the database and saves changes.
        public async Task<Student> AddAsync(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student), "Student cannot be null."); // Validate student object
            }

            try
            {
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return student;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the student.", ex);
            }
        }

        // Updates an existing student in the database.
        public async Task UpdateAsync(Student student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student), "Student cannot be null.");
            }

            try
            {
                var existingStudent = await _context.Students.FindAsync(student.StudentId);
                if (existingStudent == null)
                {
                    throw new Exception("Student not found in the database.");
                }

                // Update properties
                existingStudent.Name = student.Name;
                existingStudent.Projects = student.Projects; // Adjust this based on your requirement

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("The student you attempted to update does not exist.");
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the student.", ex);
            }
        }

        // Deletes a student by their unique identifier if they exist.
        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Invalid student ID.", nameof(id)); // Validate student ID
            }

            try
            {
                var student = await _context.Students.FindAsync(id);
                if (student != null)
                {
                    _context.Students.Remove(student);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the student.", ex);
            }
        }

        // Retrieves students associated with a specific project by project ID.
        public async Task<IEnumerable<Student>> GetByProjectIdAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                throw new ArgumentException("Invalid project ID.", nameof(projectId)); // Validate project ID
            }

            try
            {
                return await _context.Students
                    .Where(s => s.Projects.Any(p => p.ProjectId == projectId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving students by project ID.", ex);
            }
        }

        // Retrieves the count of projects associated with a specific student by student ID.
        public async Task<int> GetProjectsCountByStudentIdAsync(Guid studentId)
        {
            if (studentId == Guid.Empty)
            {
                throw new ArgumentException("Invalid student ID.", nameof(studentId)); // Validate student ID
            }

            try
            {
                var student = await _context.Students.Include(s => s.Projects).FirstOrDefaultAsync(s => s.StudentId == studentId);
                return student?.Projects.Count ?? 0; // Return project count or 0 if student not found
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while counting projects for the student.", ex);
            }
        }

        // Retrieves students by their names.
        public async Task<IEnumerable<Student>> GetStudentsByNamesAsync(IEnumerable<string> studentNames)
        {
            if (studentNames == null || !studentNames.Any())
            {
                throw new ArgumentException("Student names cannot be null or empty.", nameof(studentNames)); // Validate names input
            }

            try
            {
                return await _context.Students
                    .Where(s => studentNames.Contains(s.Name))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving students by names.", ex);
            }
        }
    }
}

