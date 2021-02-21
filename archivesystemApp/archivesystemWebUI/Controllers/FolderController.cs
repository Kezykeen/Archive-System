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
            var rootFolder = repo.FolderRepo.GetRootFolder();
            var folders = repo.FolderRepo.GetSubFolders(rootFolder.Id);
            
            FolderListViewModel model = new FolderListViewModel
            {
                Id = rootFolder.Id,
                FolderName = rootFolder.Name,
                SubFolders = folders
            };
            return View("FolderList",model);
        }

        // GET: /folder/add
        [Route("folders/add")]
        [HttpGet]
        public ActionResult Create(int id)
        {
            var data = new CreateFolderViewModel() { Name = "", ParentId = id };
            return View("CreateFolder",data);
        }

        //POST: /folder/create
        [Route("folders/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string name,int parentId)
        {
            Folder rootFolder = repo.FolderRepo.GetRootFolder();
            IEnumerable<string> folderNames = repo.FolderRepo.GetSubFolderNames(parentId);
            if(folderNames.Contains(name) || name == "Root")
            {
                ModelState.AddModelError("", $"{name} folder already exist");
                return View("CreateFolder", new CreateFolderViewModel() { Name = name, ParentId = parentId });
            }

            var folder = new Folder()
            {
                Name = name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            repo.FolderRepo.Add(folder);
            repo.FolderRepo.AddToParentFolder(parentId, folder.Id);
            repo.Save();

            if (parentId == rootFolder.Id)
                return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(GetSubFolders), new { id=parentId });
        }

       
        [Route("folders/delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var folderToDelete=repo.FolderRepo.Get(id);
            repo.FolderRepo.RecursiveDelete(folderToDelete);
            repo.Save();
            return RedirectToAction(nameof(Index));
        }

        [Route("folders/{id}")]
        [HttpGet]
        public ActionResult GetSubFolders(int id)
        {
            var folders = repo.FolderRepo.GetSubFolders(id);
            var folder = repo.FolderRepo.Get(id);
            var parentId = repo.FolderRepo.GetParentId(id);
            var model = new FolderListViewModel { FolderName=folder.Name,Id=folder.Id,SubFolders=folders , ParentId=parentId};
            return View("FolderList", model);
        }

        [HttpPost]
        [Route("folders/{id}")]
        public ActionResult BackToParent(int parentId)
        {
            var folder = repo.FolderRepo.Get(parentId);
            if (folder.Name == "Root")
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(GetSubFolders), new { id = folder.Id });
        }
    }
}

//public override void Up()
//{
//    Sql("INSERT INTO [dbo].[Folders] ( [Name], [CreatedAt], [UpdatedAt]) VALUES ( N'Root', N'2021-02-19 00:00:00', N'2021-02-19 00:00:00')");
//}

//public override void Down()
//{
//    Sql("DELETE FROM [dbo].[Folders] Where [Name]= 'Root'");
//}