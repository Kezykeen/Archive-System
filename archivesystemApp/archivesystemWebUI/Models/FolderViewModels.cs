using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class CreateFolderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public IEnumerable<AccessLevel> AccessLevels { get; set; }
        public int AccessLevelId {get;set;}

       
    }

    public class FolderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? ParentId { get; set; }

        [ForeignKey("ParentId")]
        public ICollection<Folder> Subfolders { get; set; }

        public string Path { get; set; }
    }

    public class DeleteFolderViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public int ParentId { get; set; }
    }
}