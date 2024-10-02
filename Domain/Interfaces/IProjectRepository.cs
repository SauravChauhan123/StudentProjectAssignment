using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProjectRepository
    {
        Task<Project> GetByIdAsync(Guid id);
        Task<IEnumerable<Project>> GetAllAsync(int page, int pageSize);
        Task<Project> AddAsync(Project project);
        Task UpdateProjectAsync(Project project);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Student>> GetStudentsByProjectAsync(Guid projectId);
        Task<IEnumerable<Project>> GetProjectByNamesAsync(IEnumerable<string> studentNames);
    }
}
