using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.RoleViewModels
{
    public class UsersInRoleViewModel
    {
        public string RoleName { get; set; }
        public IEnumerable<RoleMemberData> Users { get; set; }
    }

    

}