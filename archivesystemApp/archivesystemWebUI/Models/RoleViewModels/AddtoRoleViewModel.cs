using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.RoleViewModels
{
    public class AddToRoleViewModel
    {
        public IEnumerable<ApplicationRole> Roles { get; set; }
        public string UserId { get; set; }
        public string EmployeeName { get; set; }
    }
}