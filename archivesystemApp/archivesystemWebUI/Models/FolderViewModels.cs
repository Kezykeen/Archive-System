using archivesystemDomain.Entities;
using archivesystemDomain.Services;
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

        public List<FolderPath> CurrentPath { get; set; }

        public ICollection <Folder> DirectChildren { get;set; }
    }

    public class DeleteFolderViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public int ParentId { get; set; }
    }

    public class MoveItemViewModel
    {
        public int Id { get; set; }
         
        public string FileType { get; set; }

        public int NewParentFolder { get; set; }
           
    }


}