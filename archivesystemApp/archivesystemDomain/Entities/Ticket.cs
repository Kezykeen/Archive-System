using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace archivesystemDomain.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Acronym { get; set; }
        public Designation Designation { get; set; }
        public WorkFlow WorkFlow { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public Status Status { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}