using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class CreateFolderViewModel
    {
        public string Name { get; set; }
        public int ParentId { get; set; }
        public IEnumerable<AccessLevel> AccessLevels { get; set; }
        public int AccessLevelId {get;set;}

       
    }

    public class FolderListViewModel
    {
        public int Id { get; set; }
        public string FolderName { get; set; }

        public int ParentId { get; set; }

        public IEnumerable<Folder> SubFolders { get; set; }
    }
}