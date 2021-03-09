using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace archivesystemDomain.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [Index("UserIdIndex")]
        [StringLength(50)]
        public string UserId { get; set; }

        [Index("EmailIndex", IsUnique = true)]
        [StringLength(50)]
        public string Email { get; set; }

        [Index("NameIndex", IsUnique = true)]
        [StringLength(50)]
        public string Name { get; set; }

        public Gender Gender { get; set; }
        [StringLength(50)]
        public string Phone { get; set; }
        
        [Index("StaffIdIndex", IsUnique = true)]
        [StringLength(50)]
        public string StaffId { get; set; }
        public DateTime DOB { get; set; }
        public DateTime Appointed { get; set; }
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        public bool Completed { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
