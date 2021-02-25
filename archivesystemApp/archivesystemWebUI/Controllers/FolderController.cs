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
                var rootFolder = repo.FolderRepo.GetRootFolder();
                model.SubFolders = repo.SubFolderRepo.GetSubFolders(rootFolder.Id);
                Session[SessionData.FolderPath] = repo.SubFolderRepo.GetFolderPath(rootFolder.Id);
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
            return View("AddFolder",data);
        }

        //POST: /folders/create
        [Route("folders/add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string name,int parentId,int accessLevelId)
        {
            Folder rootFolder = repo.FolderRepo.GetRootFolder();
            IEnumerable<string> folderNames = repo.SubFolderRepo.GetSubFolderNames(parentId);
            if(folderNames.Contains(name) || name == "Root")
            {
                ModelState.AddModelError("", $"{name} folder already exist");
                return View("CreateFolder", new CreateFolderViewModel() { Name = name, ParentId = parentId, AccessLevelId=accessLevelId  });
            }

            var folder = new Folder{Name = name,CreatedAt = DateTime.Now,UpdatedAt = DateTime.Now };
            repo.FolderRepo.Add(folder);
            var subFolder = new SubFolder { AccessLevelId = accessLevelId, FolderId = folder.Id, ParentId = parentId };
            repo.SubFolderRepo.Add(subFolder);
            repo.Save();

            if (parentId == rootFolder.Id)
                return RedirectToAction(nameof(Index));
            return RedirectToAction(nameof(GetSubFolders), new { id=parentId });
        }

        //POST: /folders/create
        [Route("folders/delete")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,int parentId)
        {
            var folderToDelete=repo.FolderRepo.Get(id);
            List<Folder> foldersToDelete=repo.SubFolderRepo.RecursiveGetSubFolders(folderToDelete);
            repo.FolderRepo.DeleteFolders(foldersToDelete);
            repo.Save();
            return RedirectToAction(nameof(GetSubFolders),new { id=parentId});
        }

        //GET: /folders/{id}
        [Route("folders/{id}")]
        [HttpGet]
        public ActionResult GetSubFolders(int id)
        {
            var folders = repo.SubFolderRepo.GetSubFolders(id);
            var folder = repo.FolderRepo.Get(id);
            int parentId;
            if (folder.Name == "Root")
                parentId = 0;
            else 
                parentId = repo.SubFolderRepo.GetParentId(id);
            var model = new FolderListViewModel { FolderName=folder.Name,Id=folder.Id,SubFolders=folders , ParentId=parentId};

            var currentPath = (Stack<Folder>)Session[SessionData.FolderPath];

            if (currentPath == null)
            {
                Session[SessionData.FolderPath] = repo.SubFolderRepo.GetFolderPath(id);
            }
            else
            {
                if (currentPath.Peek().Id == folder.Id) { }
                else if (currentPath.Peek().Id != parentId && currentPath.Count() > 0)
                {
                    while (currentPath.Peek().Id != id)
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
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CreateFolderViewModel model)
        {
            var folder = new Folder {Name=model.Name, Id=model.Id };
            repo.FolderRepo.UpdateFolder(folder);
            var subFoleder = new SubFolder { FolderId = model.Id, AccessLevelId = model.AccessLevelId };
            repo.SubFolderRepo.Update(subFoleder);
            repo.Save();
            return RedirectToAction(nameof(GetSubFolders),new { id=model.Id });
        }


        //GET: /Folder/GetEditPartialView
        public ActionResult GetEditFolderPartialView(int id)
        {
            var subFolder = repo.SubFolderRepo.GetByFolderId(id);
            if (subFolder == null)
                return RedirectToAction("Index");

            var model = new CreateFolderViewModel
            {
                Id = id,
                AccessLevelId = subFolder.AccessLevelId,
                AccessLevels = repo.AccessLevelRepo.GetAll(),
                Name = subFolder.Folder.Name,
            };
            return PartialView("_EditFolder", model);
        }

        //GET: /Folder/GetDeleteFolderPartialView
        public ActionResult GetDeleteFolderPartialView(int id, string name)
        {
            var parentId = repo.SubFolderRepo.GetParentId(id);
            return View("_DeleteFolder", new DeleteFolderViewModel { Name=name, Id=id,ParentId=parentId});
        }
    
       
    
    
    }
}

