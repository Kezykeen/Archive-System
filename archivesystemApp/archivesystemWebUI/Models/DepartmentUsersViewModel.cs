using System.Collections.Generic;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Models
{
    public class DepartmentUsersViewModel
    {
        public IEnumerable<AppUser> Users { get; set; }
        public Department Department { get; set; }
    }
}