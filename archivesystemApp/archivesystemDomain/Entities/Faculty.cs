using System;
using System.Collections.Generic;

namespace archivesystemDomain.Entities
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Department> Departments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
