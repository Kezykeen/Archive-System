using System.Collections.Generic;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Models
{
    public class FacultyDepartmentsViewModel
    {
        public IEnumerable<Department> Departments { get; set; }
        public Faculty Faculty { get; set; }
    }
}