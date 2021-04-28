using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace archivesystemDomain.Entities
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Department> Departments { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
