using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class StudentDto
    {
        [NotMapped]
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public  List<string> ProjectNames { get; set; }= new List<string>();
    }
    public class StudentDtos
    {
        public Guid StudentId { get; set; }
        public string Name { get; set; }
        public List<Project> Project { get; set; }
    }
}
