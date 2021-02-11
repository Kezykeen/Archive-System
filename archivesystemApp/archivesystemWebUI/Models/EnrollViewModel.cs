using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class EnrollViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string StaffId { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}