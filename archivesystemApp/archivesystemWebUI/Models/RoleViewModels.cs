using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class RoleViewModel 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class EditRoleViewModel
    {
        public  string OldName { get; set; }

        public string NewName { get; set; }
    }


    public class UsersInRole
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
    }

    public class UsersInRoleViewModel
    {
        public string RoleId { get; set; }

        public IEnumerable<UsersInRole> Users { get; set; }
    }
}