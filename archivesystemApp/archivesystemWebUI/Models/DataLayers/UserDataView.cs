using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Models.DataLayers
{
    public class UserDataView
    {

        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }
        
        public string StaffId { get; set; }
        public string Appointed { get; set; }
        
        public string Department { get; set; }
        public string Role { get; set; }
        public string Completed { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedAt { get; set; }

    }

}