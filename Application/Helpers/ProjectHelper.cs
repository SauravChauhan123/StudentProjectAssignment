using Domain.Entities;
using Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class ProjectHelper
    {
        private readonly IStudentRepository _studentRepository;

        public ProjectHelper(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
        }

        /// <summary>
        /// Adds new students by their names if they do not already exist in the database.
        /// </summary>
        /// <param name="studentNames">The list of student names to be added.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddNewStudentsIfNeededAsync(IEnumerable<string> studentNames)
        {
            var students = await _studentRepository.GetStudentsByNamesAsync(studentNames);
            var existingStudentNames = students.Select(s => s.Name).ToHashSet(); // Create a HashSet for faster lookups

            // Handle logic for adding new students by name
            foreach (var studentName in studentNames)
            {
                if (!existingStudentNames.Contains(studentName))
                {
                    var newStudent = new Student
                    {
                        Name = studentName,
                        Projects = new List<Project>() // Initialize a new list for projects
                    };
                    await _studentRepository.AddAsync(newStudent);
                }
            }
        }
    }

}
