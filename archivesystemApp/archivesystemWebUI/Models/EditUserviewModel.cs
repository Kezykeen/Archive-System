using archivesystemDomain.Entities;
using System.Collections.Generic;

namespace archivesystemWebUI.Models
{
    public class EditUserViewModel
    {
        public CodeStatus RegenerateCode { get; set; }
        public AccessDetails AccessDetails { get; set; }
        public IEnumerable<AccessLevel> AccessLevels { get; set; }
    }
}