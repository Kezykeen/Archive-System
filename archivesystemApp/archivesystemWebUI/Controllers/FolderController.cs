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

        // GET: /folders
        [Route("folders")]
        public ActionResult Index()
        {
            FolderListViewModel model = new FolderListViewModel(); 
            if (string.IsNullOrEmpty(Request.QueryString["search"]))
            {
                var rootFolder = repo.FolderRepo.GetRootWithSubfolder();
                var folderPath= new Stack<Folder>();
                folderPath.Push(rootFolder);
                Session[SessionData.FolderPath] = folderPath;
                model.Id = rootFolder.Id;
                model.SubFolders = rootFolder.Subfolders;
                model.FolderName = "Root";
                Session[SessionData.IsSearchRequest] = false;
            }
            else
            {
                var searchParam = Request.QueryString["search"];
                model.SubFolders = repo.FolderRepo.GetMatchingFolders(searchParam);
                Session[SessionData.FolderPath] = null;
                Session[SessionData.IsSearchRequest] = true;
                
            }
            return View("FolderList",model);
        }

        // GET: /folders/add
        [Route("folders/add")]
        [HttpGet]
        public ActionResult Create(int id)
        {
            var accessLevels = repo.AccessLevelRepo.GetAll();
            var data = new CreateFolderViewModel() { Name = "", ParentId = id, AccessLevels=accessLevels };
            return PartialView("_CreateFolder",data);
        }

        //POST: /folders/create
        [Route("folders/add")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Create(string name,int parentId,int accessLevelId)
        {
            Folder rootFolder = repo.FolderRepo.GetRootFolder();
            var parentFolder = repo.FolderRepo.GetFolder(parentId);
            var parentSubFolderNames = parentFolder.Subfolders.Select(x => x.Name);
            if(parentSubFolderNames.Contains(name) || name == "Root")
            {
                return new HttpStatusCodeResult(400);
            }
            if (name.Contains(",") || name.Contains("#"))
                return new HttpStatusCodeResult(403);

            var folder = new Folder { Name = name, CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now, ParentId = parentId,
                AccessLevelId = accessLevelId,
                IsDeletable = true
            };
            repo.FolderRepo.Add(folder);
            folder.Path = parentFolder.Path + $",{name}#{folder.Id}";
            repo.Save();

            repo.FolderRepo.AddFolderPath(folder.Id);
            
            return new HttpStatusCodeResult(200); 
        }

        //POST: /folders/create
        [Route("folders/delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,int parentId)
        {
            var folderToDelete=repo.FolderRepo.Get(id);
            repo.FolderRepo.Remove(folderToDelete);
            repo.Save();
            return RedirectToAction(nameof(GetSubFolders),new { id=parentId});
        }

        //GET: /folders/{id}
        [Route("folders/{id}")]
        [HttpGet]
        public ActionResult GetSubFolders(int id)
        {
            var folder = repo.FolderRepo.GetFolder(id);
            if (folder.ParentId == null)
                return RedirectToAction(nameof(Index));
           
            var model = new FolderListViewModel { FolderName=folder.Name,Id=folder.Id,SubFolders=folder.Subfolders , ParentId=(int)folder.ParentId};
            AddCurrentFolderPath(folder);
            Session[SessionData.IsSearchRequest] = false;
            return View("FolderList", model);
        }

        //POST: /folders/{id}
        [HttpPost]
        [Route("folders/{id}")]
        public ActionResult BackToParent(int parentId)
        {
            var folder = repo.FolderRepo.Get(parentId);
            if (folder.Name == "Root")
                return RedirectToAction(nameof(Index));
            var currentPath = (Stack<Folder>)Session[SessionData.FolderPath];
            currentPath.Pop(); 
            Session[SessionData.FolderPath] = currentPath;
            return RedirectToAction(nameof(GetSubFolders), new { id = folder.Id });
        }

        //POST: /Folder/Edit
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Edit(CreateFolderViewModel model)
        {
            
            var folder = new Folder {Name=model.Name, Id=model.Id ,AccessLevelId=model.AccessLevelId};
            repo.FolderRepo.UpdateFolder(folder);
            repo.Save();
            return new HttpStatusCodeResult(200);
        }


        //GET: /Folder/GetEditPartialView
        public ActionResult GetEditFolderPartialView(int id)
        {
            var folder = repo.FolderRepo.Get(id);
            if (folder == null)
                return RedirectToAction("Index");

            var model = new CreateFolderViewModel
            {
                Id = id,
                AccessLevelId = (int)folder.AccessLevelId,
                AccessLevels = repo.AccessLevelRepo.GetAll(),
                Name = folder.Name,
            };
            return PartialView("_EditFolder", model);
        }

        //GET: /Folder/GetDeleteFolderPartialView
        public ActionResult GetDeleteFolderPartialView(int id)
        {
            var folder = repo.FolderRepo.Get(id);
            return PartialView("_DeleteFolder", new DeleteFolderViewModel { 
                Name = folder.Name, Id = id, ParentId =(int)folder.ParentId });
        }
    
       
        private void AddCurrentFolderPath(Folder folder)
        {
            var currentPath = (Stack<Folder>)Session[SessionData.FolderPath];
            if (currentPath == null)
            {
                Session[SessionData.FolderPath] = repo.FolderRepo.GetFolderPath(folder.Id);
            }
            else
            {
                if (currentPath.Peek().Id == folder.Id) { }
                else if (currentPath.Peek().Id != (int)folder.ParentId && currentPath.Count() > 0)
                {
                    while (currentPath.Peek().Id != folder.Id)
                    {
                        currentPath.Pop();
                    }
                    Session[SessionData.FolderPath] = currentPath;
                }

                else
                {
                    currentPath.Push(folder);
                    Session[SessionData.FolderPath] = currentPath;
                }

            }
        }
    
        
    }
}

