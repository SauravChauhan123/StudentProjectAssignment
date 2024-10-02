﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public string Name { get; set; } // Should be unique
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
