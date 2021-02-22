using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Entities
{
    public class SubFolder
    {
        public int Id { get; set; }

        [Required]
        public int ParentId { get; set; }
        public int FolderId { get; set; }
        public Folder Folder {get;set;}
        public AccessLevel AccessLevel { get; set; }
        public int AccessLevelId { get; set; }
    }
}
 