using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Entities
{
    public class AccessDetails
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } 
        public string AccessLevel { get; set; }
        public string AccessCode { get; set; }
        public Status Status { get; set; }
    }

}
