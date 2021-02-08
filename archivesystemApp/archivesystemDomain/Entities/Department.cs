using System;

namespace archivesystemDomain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}