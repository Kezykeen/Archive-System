﻿using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class CreateFolderViewModel
    {
        public string Name { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    }
}