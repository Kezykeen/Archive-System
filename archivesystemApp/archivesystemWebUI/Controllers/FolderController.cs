using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
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

        // GET: /folder
        [Route("folders")]
        public ActionResult Index()
         {
            var folders = repo.FolderRepo.GetAllEager();
            if (folders.Count() == 0)
                return View("FolderList", new List<FolderListViewModel>());
            var data = AutoMapFolder(folders);
            return View("FolderList",data);
        }

        // GET: /folder/add
        [Route("folders/add")]
        [HttpGet]
        public ActionResult Create()
        {
            var data = new CreateFolderViewModel() { Name = "", Departments = repo.DeptRepo.GetAll() };
            return View("CreateFolder",data);
        }

        //POST: /folder/create
        [Route("folders/add")]
        [HttpPost]
        public ActionResult Create(string name,int departmentId)
        {
            var folder = new Folder()
            {
                Name = name,
                DepartmentId = departmentId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            repo.FolderRepo.Add(folder);
            repo.Save();
            return RedirectToAction("Index");
        }

        private IEnumerable<FolderListViewModel> AutoMapFolder(IEnumerable<Folder> folders)
        {
            List<FolderListViewModel> model = new List<FolderListViewModel>();
            foreach (Folder folder in folders)
            {
                model.Add(new FolderListViewModel()
                {
                    DepartmentName = folder.Department.Name,
                    Id = folder.Id,
                    FolderName = folder.Name
                });
            }
            return model;
        }
    }
}