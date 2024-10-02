using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetByIdAsync(Guid id);
        Task<IEnumerable<Student>> GetAllAsync(int page, int pageSize);
        Task<Student> AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Student>> GetByProjectIdAsync(Guid projectId);
        Task<int> GetProjectsCountByStudentIdAsync(Guid studentId);
        Task<IEnumerable<Student>> GetStudentsByNamesAsync(IEnumerable<string> studentNames);
    }
}
