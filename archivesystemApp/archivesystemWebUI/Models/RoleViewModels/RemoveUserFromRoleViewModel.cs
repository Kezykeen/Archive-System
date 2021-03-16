using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.RoleViewModels
{
    public class RemoveUserFromRoleViewModel
    {
        public string RoleName { get; set; }

        public string UserId { get; set; }

        public string EmployeeName { get; set; }
    }
}