using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderViewModels;
using archivesystemWebUI.Repository;
using archivesystemWebUI.Services;
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
        private readonly IUnitOfWork repo;

        private IFolderService _service { get; set; }
       
        public FolderController(IUnitOfWork unitOfWork, IFolderService service) 
        {
            repo = unitOfWork;
            _service = service;
        }

        // GET: /folders
        [Route("folders")]
        public ActionResult Index(string search = null, string returnUrl="/folders")
        {
            FolderPageViewModel model = GetRootViewModel(returnUrl, search);
            CheckForUserAccessCode(out bool hasCorrectAccessCode, out double timeSinceLastRequest);
            model.CloseAccessCodeModal = hasCorrectAccessCode && timeSinceLastRequest <=LOCKOUT_TIME;
            model.Files = model.CloseAccessCodeModal ? model.Files : null;
            ViewBag.AllowCreateFolder = false;
            ViewBag.ErrorMessage = TempData["errorMessage"];
            return View("FolderList", model);
        }

        // GET: /folders/add
        [Route("folders/add")]
        [HttpGet]
        public ActionResult Create(int id)
        {
            return PartialView("_CreateFolder", _service.GetCreateFolderViewModel(id));
        }

        //POST: /folders/add
        [Route("folders/add")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Create(SaveFolderViewModel model)
        {
            var result = _service.TrySaveFolder(model);

            if (result == FolderActionResult.Success) return new HttpStatusCodeResult(200);

            if (result==FolderActionResult.AlreadyExist)  return new HttpStatusCodeResult(400);

            if (result == FolderActionResult.InvalidAccessLevel) return new HttpStatusCodeResult(403);

            if (result== FolderActionResult.MaxFolderDepthReached) return new HttpStatusCodeResult(404);

            return new HttpStatusCodeResult(500);
        }

        [Route("folders/move")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult MoveItem(MoveItemViewModel model)
        { 
            var result= _service.MoveFolder(model);

            if (result== FolderActionResult.InvalidModelState) return new HttpStatusCodeResult(400);

            if (result== FolderActionResult.Prohibited) return new HttpStatusCodeResult(405);

            if (result== FolderActionResult.Success) return new HttpStatusCodeResult(200);

            return new HttpStatusCodeResult(500);
        }

        //POST: /folders/create
        [Route("folders/delete")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var result = _service.DeleteFolder(id);
            if (result==FolderActionResult.Success) return new HttpStatusCodeResult(204);

            if (result == FolderActionResult.Prohibited) return new HttpStatusCodeResult(400);

            return new HttpStatusCodeResult(500);  
        }

        //GET: /folders/{id}
        [Route("folders/{folderId}")]
        [HttpGet]
        public ActionResult GetFolder(int folderId)
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

            return View("FolderList", GetViewModelForView(folder, folderId));
        }

        //POST: /folders/{parentId}
        [HttpPost]
        [Route("folders/{parentId}")]
        public ActionResult BackToParent(int parentId)
        {
            return RedirectToAction(nameof(GetFolder), new { folderId = parentId });
        }

        //POST: /Folder/Edit
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Edit(CreateFolderViewModel model)
        {
            var result = _service.Edit(model);
            if (result == FolderActionResult.InvalidModelState) return new HttpStatusCodeResult(400);

            if (result == FolderActionResult.Success) return new HttpStatusCodeResult(200);

            return new HttpStatusCodeResult(500);
        }


        //GET: /Folder/GetEditPartialView
        public ActionResult GetEditFolderPartialView(int id)
        {
            var folder = _service.GetFolder(id);
            if (folder.IsRestricted)
                return new HttpStatusCodeResult(403);

            if (folder == null)
                return new HttpStatusCodeResult(400);

            var model = new CreateFolderViewModel
            {
                Id = id,
                AccessLevelId = (int)folder.AccessLevelId,
                AccessLevels = _service.GetCurrentUserAllowedAccessLevels(),
                Name = folder.Name,
            };
            return PartialView("_EditFolder", model);
        }

        //GET: /Folder/GetDeleteFolderPartialView
        public ActionResult GetDeleteFolderPartialView(int id)
        {
            var folder = _service.GetFolder(id);
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
            if (accessCode != _service.GetCurrentUserAccessCode())
                return new HttpStatusCodeResult(400);

            Session[SessionData.IsAccessValidated] = true;
            Session[SessionData.LastVisit] = DateTime.Now;
            return new HttpStatusCodeResult(200);
        }


        #region Private Methods
        private FolderPageViewModel GetRootViewModel(string returnUrl, string search = null)
        {

            FolderPageViewModel model = new FolderPageViewModel();
            if (string.IsNullOrEmpty(search))
            {
                _service.GetUserData(out AppUser user, out int userAccessLevel);
                var rootFolder = _service.GetRootFolder();
                model.Name = rootFolder.Name;
                model.CurrentPath = new List<FolderPath> { new FolderPath { Id = rootFolder.Id, Name = "Root" } };
                model.Id = rootFolder.Id; model.ReturnUrl = returnUrl;
                model.DirectChildren = HttpContext.User.IsInRole("Admin") ?
                    rootFolder.Subfolders : rootFolder.Subfolders.Where(x => x.FacultyId == user.Department.FacultyId).ToList();
                return model;
            }

            model.DirectChildren = _service.GetFoldersThatMatchName(search).ToList();
            model.CurrentPath = new List<FolderPath>();
            model.CloseAccessCodeModal = true;
            model.Id = 0;

            return model;
        }

        private void VerifyThatUserHasAccessToFolder(out bool hasAuthorizedAccessToFolder, Folder folder)
        {
            hasAuthorizedAccessToFolder = _service.DoesUserHasAccessToFolder(folder);
            TempData["errorMessage"] =
                hasAuthorizedAccessToFolder ? null : $"You are not authorized to view {folder.Name} folder";

        }

        private void CheckForUserAccessCode(out bool hasCorrectAccessCode, out double timeSinceLastRequest)
        {
            hasCorrectAccessCode =
               Session[SessionData.IsAccessValidated] != null && (bool)Session[SessionData.IsAccessValidated];
            var lastVisitTime = Session[SessionData.LastVisit] ?? new DateTime();
            timeSinceLastRequest = (DateTime.Now - (DateTime)lastVisitTime).TotalMinutes;
        }

        private FolderPageViewModel GetViewModelForView( Folder folder, int folderId)
        {
            var folderpath = _service.GetFolderPath(folderId);
            var model = Mapper.Map<FolderPageViewModel>(folder);
            model.CloseAccessCodeModal = true;
            model.CurrentPath = folderpath;
            ViewBag.AllowCreateFolder = true;
            Session[SessionData.LastVisit] = DateTime.Now;
            return model;
        }

        private void GetFolder(out Folder folder, int folderId)
        {
            folder = _service.GetFolder(folderId);
        }

       
        #endregion
    }
}

