using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class StudentHelper
    {
        private readonly IProjectRepository _projectRepo;

        public StudentHelper(IProjectRepository projectRepo)
        {
            _projectRepo = projectRepo ?? throw new ArgumentNullException(nameof(projectRepo));
        }

        /// <summary>
        /// Fetches or creates projects based on the provided project names.
        /// </summary>
        /// <param name="projectNames">The list of project names to check or create.</param>
        /// <returns>A list of existing or newly created projects.</returns>
        public async Task<List<Project>> FetchOrCreateProjectsAsync(IEnumerable<string> projectNames)
        {
            // Fetch existing projects from the repository
            var existingProjects = await _projectRepo.GetProjectByNamesAsync(projectNames);
            var existingProjectNames = existingProjects.Select(p => p.Name).ToHashSet();

            // Create new projects if they do not exist
            foreach (var projectName in projectNames)
            {
                if (!existingProjectNames.Contains(projectName))
                {
                    var newProject = new Project
                    {
                        Name = projectName,
                        Students = new List<Student>() // Initialize a new list for students
                    };
                    await _projectRepo.AddAsync(newProject);
                }
            }

            // Re-fetch existing projects to return the updated list
            return (await _projectRepo.GetProjectByNamesAsync(projectNames)).ToList(); // Convert to List here
        }
    }
}
