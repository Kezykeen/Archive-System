using System;
using System.Collections.Generic;
using System.Text;

namespace archivesystemDomain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string StaffId { get; set; }
        public DateTime? DOB { get; set; }
        public MaritalStatus MaritalStatus { get; set; }
        public string Address { get; set; }
        public Department Department { get; set; }
        public int DepartmentId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
