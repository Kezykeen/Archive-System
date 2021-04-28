using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.RoleViewModels
{
    public class AddOrEditRoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string NewName { get; set; }
    }
}