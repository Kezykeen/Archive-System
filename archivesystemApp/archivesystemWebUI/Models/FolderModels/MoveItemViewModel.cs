﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.FolderModels
{
    public class MoveItemViewModel
    {
        public int Id { get; set; }

        public int NewParentFolderId { get; set; }

    }
}