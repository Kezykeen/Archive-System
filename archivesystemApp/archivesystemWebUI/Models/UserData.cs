using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class UserData
    {
        public AppUser User { get; set; }
        public int UserAccessLevel { get; set; }
    }
}