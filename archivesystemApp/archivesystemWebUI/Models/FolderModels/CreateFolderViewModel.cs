using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.FolderModels
{
    public class CreateFolderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public IEnumerable<AccessLevel> AccessLevels { get; set; }
        public int AccessLevelId { get; set; }


    }
}