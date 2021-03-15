using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderViewModels;
using archivesystemWebUI.Repository;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace archivesystemWebUI.Controllers
{

    [Authorize]
    public class FolderController : Controller
    {
        private const byte LOCKOUT_TIME = 1; //lockout user after last request exceeds lockout time in minutes
        private bool isUserAccessVerified;

        private IUnitOfWork repo { get; set; }
        private IFolderService _service { get; set; }
        private bool IsUserAccessVerified { get { return isUserAccessVerified; } set { isUserAccessVerified = value; } }


        public FolderController(IUnitOfWork repo,IFolderService service)
        {
            this.repo = repo;
            _service = service;
        }

        // GET: /folders
        [Route("folders")]
        public ActionResult Index(string search = null, string returnUrl="/folders")
        {
            FolderPageViewModel model = GetRootViewModel(returnUrl, search);
            model.CloseAccessCodeModal = returnUrl == "/folders" ? true : IsUserAccessVerified;
            ViewBag.AllowCreateFolder = false;
            ViewBag.ErrorMessage = TempData["errorMessage"];
            return View("FolderList", model);
        }

        // GET: /folders/add
        [Route("folders/add")]
        [HttpGet]
        public ActionResult Create(int id)
        {
            var accessLevels = repo.AccessLevelRepo.GetAll();
            var parentFolder = repo.FolderRepo.Get(id);
            accessLevels = accessLevels.Where(x => x.Id >= parentFolder.AccessLevelId);
            var data = new CreateFolderViewModel() { Name = "", ParentId = id, AccessLevels = accessLevels };
            return PartialView("_CreateFolder", data);
        }

        //POST: /folders/add
        [Route("folders/add")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Create(string name, int parentId, int accessLevelId)
        {
            Folder rootFolder = repo.FolderRepo.GetRootFolder();
            var parentFolder = repo.FolderRepo.GetFolderWithSubFolders(parentId);
            var parentSubFolderNames = parentFolder.Subfolders.Select(x => x.Name);
            if (parentSubFolderNames.Contains(name) || name == "Root")
                return new HttpStatusCodeResult(400);
            if (parentFolder.AccessLevelId > accessLevelId)
                return new HttpStatusCodeResult(403);

            var parentFolderPath = repo.FolderRepo.GetFolderPath(parentId);
            var parentFolderCurrentFolderDepth = parentFolderPath.Count();
            if (parentFolderCurrentFolderDepth >= (int)AllowableFolderDepth.Max)
                return new HttpStatusCodeResult(404);

            var folder = new Folder
            {
                Name = name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ParentId = parentId,
                AccessLevelId = accessLevelId,
                IsRestricted = false,
                DepartmentId = parentFolder.DepartmentId,
            };
            repo.FolderRepo.Add(folder);
            repo.Save();

            return new HttpStatusCodeResult(200);
        }

        [Route("folders/move")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult MoveItem(MoveItemViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new HttpStatusCodeResult(400);
                if (model.Id == model.NewParentFolder)
                    return new HttpStatusCodeResult(405);
                if (model.FileType == "folder")
                    repo.FolderRepo.MoveFolder(model.Id, model.NewParentFolder);
                repo.Save();
                return new HttpStatusCodeResult(200);
            }

            catch (Exception e)
            {
                if (e.Message.Contains("folder already exist"))
                    return new HttpStatusCodeResult(403);
                return new HttpStatusCodeResult(500);
            }

        }

        //POST: /folders/create
        [Route("folders/delete")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Delete(int id, int parentId)
        {
            var folderToDelete = repo.FolderRepo.Get(id);
            if (folderToDelete.IsRestricted)
                return new HttpStatusCodeResult(400);

            repo.FolderRepo.DeleteFolder(folderToDelete.Id);
            repo.Save();
            return new HttpStatusCodeResult(204);
        }

        //GET: /folders/{id}
        [Route("folders/{folderId}")]
        [HttpGet]
        public ActionResult GetFolderSubFolders(int folderId)
        {
            GetFolder(out Folder folder, folderId);
            if (folder.Name == "Root")
                return RedirectToAction(nameof(Index));

            if (!HttpContext.User.IsInRole("Admin"))
            {
                CheckForUserAccessCode(out bool hasCorrectAccessCode, out double timeSinceLastRequest);
                if (!hasCorrectAccessCode || timeSinceLastRequest > LOCKOUT_TIME)
                    return RedirectToAction(nameof(Index), new { returnUrl = $"/folders/{folderId}" });

                VerifyThatUserHasAccessToFolder(out bool hasAuthorizedAccessToFolder, folder);
                if (!hasAuthorizedAccessToFolder)
                    return RedirectToAction(nameof(Index));
            }

            GetFolderPageViewModel(out FolderPageViewModel model, folder, folderId);
            return View("FolderList", model);
        }

        //POST: /folders/{parentId}
        [HttpPost]
        [Route("folders/{parentId}")]
        public ActionResult BackToParent(int parentId)
        {
            return RedirectToAction(nameof(GetFolderSubFolders), new { folderId = parentId });
        }

        //POST: /Folder/Edit
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Edit(CreateFolderViewModel model)
        {

            var folder = new Folder { Name = model.Name, Id = model.Id, AccessLevelId = model.AccessLevelId };
            repo.FolderRepo.UpdateFolder(folder);
            repo.Save();
            return new HttpStatusCodeResult(200);
        }


        //GET: /Folder/GetEditPartialView
        public ActionResult GetEditFolderPartialView(int id)
        {
            var folder = repo.FolderRepo.Get(id);
            if (folder.IsRestricted)
                return new HttpStatusCodeResult(403);

            if (folder == null)
                return new HttpStatusCodeResult(400);

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
            if (folder == null)
                return new HttpStatusCodeResult(400);
            if (folder.IsRestricted)
                return new HttpStatusCodeResult(403);
            return PartialView("_DeleteFolder", new DeleteFolderViewModel
            {
                Name = folder.Name,
                Id = id,
                ParentId = (int)folder.ParentId
            });
        }

        //GET: /Folder/GetConfirmItemMovePartialView
        public ActionResult GetConfirmItemMovePartialView()
        {

            ViewBag.ItemName = Request.QueryString["itemName"];
            ViewBag.CurrentFolder = Request.QueryString["currentFolder"];
            return PartialView("_ConfirmItemMove");
        }
         
        //GET: /Folder/VerifyAccessCode
        public ActionResult VerifyAccessCode(string accessCode)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var user = repo.EmployeeRepo.GetEmployeeByUserId(userId);
            var userAccessCode = repo.AccessDetailsRepo.GetByEmployeeId(user.Id).AccessCode;

            if (accessCode != userAccessCode)
                return new HttpStatusCodeResult(400);

            Session[SessionData.IsAccessValidated] = true;
            Session[SessionData.LastVisit] = DateTime.Now;
            return new HttpStatusCodeResult(200);
        }

        private FolderPageViewModel GetRootViewModel(string search = null)
        {
            FolderPageViewModel model = new FolderPageViewModel();
            if (string.IsNullOrEmpty(search))
            {
                var rootFolder = repo.FolderRepo.GetRootWithSubfolder();
                model.Name = "Root";
                model.DirectChildren = rootFolder.Subfolders;
                model.CurrentPath = new List<FolderPath> { new FolderPath { Id = rootFolder.Id, Name = "Root" } };
                model.Id = rootFolder.Id;
            }
            else
            {
                model.DirectChildren = repo.FolderRepo.GetFoldersThatMatchName(search);
                model.CurrentPath = new List<FolderPath>();
                model.Id = 0;
            }
            return model;
        }

        private bool AccessNotGranted(Folder folder)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var user = repo.EmployeeRepo.GetEmployeeByUserId(userId);
            var userAccessLevel = repo.AccessDetailsRepo.GetByEmployeeId(user.Id).AccessLevelId;

            if (folder.FacultyId != null && user.Department.FacultyId != folder.FacultyId)
                return true;

            if (folder.FacultyId == null)
            {
                if (folder.AccessLevelId > userAccessLevel || folder.DepartmentId != user.DepartmentId)
                    return true;
            }
            folder.Subfolders = folder.Subfolders.Where(x => x.AccessLevelId <= userAccessLevel).ToList();
            return false;
        }


        #region Private Methods
        private FolderPageViewModel GetRootViewModel(string returnUrl, string search = null)
        {

            FolderPageViewModel model = new FolderPageViewModel();
            if (string.IsNullOrEmpty(search))
            {
                _service.GetUserData(out Employee user, out int userAccessLevel);
                var rootFolder = _service.GetRootWithSubfolder();
                model.Name = rootFolder.Name;
                model.CurrentPath = new List<FolderPath> { new FolderPath { Id = rootFolder.Id, Name = "Root" } };
                model.Id = rootFolder.Id; model.ReturnUrl = returnUrl;
                model.DirectChildren = HttpContext.User.IsInRole("Admin") ?
                    rootFolder.Subfolders : rootFolder.Subfolders.Where(x => x.FacultyId == user.Department.FacultyId).ToList();
                return model;
            }

            model.DirectChildren = repo.FolderRepo.GetFoldersThatMatchName(search);
            model.CurrentPath = new List<FolderPath>();
            model.CloseAccessCodeModal = true;
            model.Id = 0;

            return model;
        }

        private void SaveFolder(Folder parentFolder, List<FolderPath> parentFolderPath, string folderName, int? accessLevelId)
        {
            _service.SaveFolder(parentFolder, parentFolderPath, folderName, accessLevelId);
        }

        private void GetParentFolderData(out Folder parentFolder, out List<FolderPath> parentFolderPath, int parentId)
        {
            parentFolder = repo.FolderRepo.GetFolderWithSubFolders(parentId);
            parentFolderPath = repo.FolderRepo.GetFolderPath(parentId);
        }

        private void VerifyThatUserHasAccessToFolder(out bool hasAuthorizedAccessToFolder, Folder folder)
        {
            hasAuthorizedAccessToFolder = _service.DoesUserHasAccessToFolder(folder);
            TempData["errorMessage"] =
                hasAuthorizedAccessToFolder ? null : $"You are not authorized to view {folder.Name} folder";

        }

        private void CheckForUserAccessCode(out bool hasCorrectAccessCode, out double timeInMinutes)
        {
            hasCorrectAccessCode =
               Session[SessionData.IsAccessValidated] != null && (bool)Session[SessionData.IsAccessValidated];
            var lastVisitTime = Session[SessionData.LastVisit] ?? new DateTime();
            timeInMinutes = (DateTime.Now - (DateTime)lastVisitTime).TotalMinutes;
            if (!hasCorrectAccessCode)
                IsUserAccessVerified = false;
        }

        private void GetFolderPageViewModel(out FolderPageViewModel model, Folder folder, int folderId)
        {
            var folderpath = _service.GetFolderPath(folderId);
            model = Mapper.Map<FolderPageViewModel>(folder);
            model.CloseAccessCodeModal = true;
            model.CurrentPath = folderpath;
            ViewBag.AllowCreateFolder = true;
            Session[SessionData.LastVisit] = DateTime.Now;
        }

        private void GetFolder(out Folder folder, int folderId)
        {
            folder = repo.FolderRepo.GetFolderWithSubFolders(folderId);
        }
        #endregion
    }
}

