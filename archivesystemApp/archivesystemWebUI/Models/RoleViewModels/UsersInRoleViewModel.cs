using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.RoleViewModels
{
    public class UsersInRoleViewModel
    {
        public string RoleId { get; set; }

        public IEnumerable<Employee> Users { get; set; }
    }
}