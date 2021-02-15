using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Controllers
{
    public class FolderController : Controller
    {
        private IUnitOfWork repo { get; set; }
        public FolderController(IUnitOfWork repo)
        {
            this.repo = repo;
        }

        //// GET: Folder
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [Route("folders/add")]
        [HttpGet]
        public ActionResult Create()
        {
            var data = new CreateFolderViewModel() { Name = "", Departments = repo.DeptRepo.GetAll() };
            return View("CreateFolder",data);
        }
    }
}