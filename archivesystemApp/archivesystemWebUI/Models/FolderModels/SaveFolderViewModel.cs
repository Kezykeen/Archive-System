using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.FolderModels
{
    public class SaveFolderViewModel
    {
        public string Name { get; set; } 
        public int? AccessLevelId { get; set; }
        public int ParentId { get; set; }
    }
}