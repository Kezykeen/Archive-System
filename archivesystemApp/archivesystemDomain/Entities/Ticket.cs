using System;
using System.Collections.Generic;

namespace archivesystemDomain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Designation Designation { get; set; }
        public IEnumerable<ApplicationRole> AuthorizeRoles { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}