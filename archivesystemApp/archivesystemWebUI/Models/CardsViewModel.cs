using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class CardsViewModel
    {
        public IEnumerable<AppUser> Appusers { get; set; }
        public IEnumerable<Department> Departments { get; set; }
        public IEnumerable<AccessDetail> AccessDetails { get; set; }
        public IEnumerable<Faculty> Faculties { get; set; }
        public IEnumerable<ApplicationRole> Roles { get; set; }
    }
}