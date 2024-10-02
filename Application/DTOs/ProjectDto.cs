using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for Project information.
    /// </summary>
    public class ProjectDto
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the project.
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the list of student names associated with the project.
        /// </summary>
        public List<string> StudentNames { get; set; } = new List<string>();

        #endregion
    }
}

