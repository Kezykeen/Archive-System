using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.FolderModels
{
    public class DeleteFolderViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }

        public int ParentId { get; set; }
    }

}