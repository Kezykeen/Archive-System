using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.FolderModels
{
    public class EditFolderViewModel
    {
        public string Name { get; set; }
        public int? AccessLevelId { get; set; }
    }
}