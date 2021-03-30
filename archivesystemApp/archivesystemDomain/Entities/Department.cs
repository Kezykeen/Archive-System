using System;
using System.ComponentModel.DataAnnotations;

namespace archivesystemDomain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [Display(Name = "Faculty")]
        public int FacultyId { get; set; }

        public Faculty Faculty { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}