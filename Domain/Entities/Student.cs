using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Student
    {
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
