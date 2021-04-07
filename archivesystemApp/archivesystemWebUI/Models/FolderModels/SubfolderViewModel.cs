using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace archivesystemWebUI.Models.FolderModels
{
    public class SubfolderViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SubfolderViewModelResult
    {
        public HttpStatusCode Status { get; set; }
        public IEnumerable<SubfolderViewModel> Data { get; set; }
    }
}