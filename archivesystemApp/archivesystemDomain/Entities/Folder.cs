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

        [Required]
        public string Name { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public Folder Parent { get; set; }

        public ICollection<Folder> Subfolders { get; set; }


        public ICollection<File> Files { get; set; }

        [ForeignKey("AccessLevel")]
        public int? AccessLevelId { get; set; }

        public AccessLevel AccessLevel { get; set; }
 
        [Required]
        public DateTime CreatedAt {get;set;}

        [Required]
        public DateTime UpdatedAt {get;set;}


    }
}
