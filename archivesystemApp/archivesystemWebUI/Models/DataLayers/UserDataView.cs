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
        
        public string TagId { get; set; }
        
        public string Department { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Completed { get; set; }
        public string UpdatedAt { get; set; }
        public string CreatedAt { get; set; }

    }

}