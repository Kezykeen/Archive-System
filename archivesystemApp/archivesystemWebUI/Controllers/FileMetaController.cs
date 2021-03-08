using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Controllers
{
    public class FileMetaController : Controller
    {
        // GET: FileMeta
        public ActionResult New()
        {
            return PartialView(new FileMetaVm());
        }


        public ActionResult Edit()
        {
            return View();
        }
    }
}