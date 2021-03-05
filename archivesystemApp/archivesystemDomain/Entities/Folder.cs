using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Entities
{
    public class Folder
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
       
        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<Folder> Subfolders { get; set; }

        public int? FileId { get; set; }
        public File File { get; set; }

        public int? AccessLevelId { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public ICollection<File> Files { get; set; }
      
        public bool IsRestricted { get; set;}
 
        public DateTime CreatedAt {get;set;}

        public DateTime UpdatedAt {get;set;}


    }
}
