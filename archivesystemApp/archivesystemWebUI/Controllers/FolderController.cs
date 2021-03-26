using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderModels;
using archivesystemWebUI.Repository;
using archivesystemWebUI.Services;
using AutoMapper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;


namespace archivesystemWebUI.Controllers
{

    [Authorize]
    public class FolderController : Controller
    {
        private const byte LOCKOUT_TIME = 1; //lockout user after last request exceeds lockout time in minutes

        private readonly IRoleService _roleService;
        private readonly IAccessCodeGenerator _accessCodeGenerator;
        private readonly IFolderService _service;
       
        public FolderController(IRoleService roleService, IFolderService service, IAccessCodeGenerator accessCodeGenerator) 
        {
            _accessCodeGenerator = accessCodeGenerator;
            _service = service;
            _roleService = roleService;
        }

        // GET: /folders
        [Route("folders")]
        public ActionResult Index(string search = null, string returnUrl="/folders")
        {
            FolderPageViewModel model = GetRootViewModel(returnUrl, search);
            if (!HttpContext.User.IsInRole(RoleNames.Admin))
            {
                if (string.IsNullOrEmpty(model.Name) && search ==null)
                {
                    TempData[GlobalConstants.userHasNoAccesscode] = true;
                    return RedirectToAction("Index", "Home");
                }
                CheckForUserAccessCode(out bool hasCorrectAccessCode, out double timeSinceLastRequest);
                model.CloseAccessCodeModal = hasCorrectAccessCode && timeSinceLastRequest <= LOCKOUT_TIME;
            }
            else
                model.CloseAccessCodeModal = true;

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
            CreateFolderViewModel viewModel;
            if(HttpContext.User.IsInRole(RoleNames.Admin))
                viewModel=_service.GetCreateFolderViewModel(id, HttpContext.User.Identity.GetUserId(),userIsAdmin:true);
            else
                viewModel = _service.GetCreateFolderViewModel(id, HttpContext.User.Identity.GetUserId());
            return PartialView("_CreateFolder",viewModel );
        }

        //POST: /folders/add
        [Route("folders/add")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Create(SaveFolderViewModel model)
        {
            if (!HttpContext.User.IsInRole(RoleNames.Admin))
            {
                var userIsnotPermitted = IsCreateActionForbidden(_service.GetFolder(model.ParentId));
                if (userIsnotPermitted)
                    return new HttpStatusCodeResult(403);
            }

            var result = _service.SaveFolder(model);
            if (result == FolderServiceResult.Success) return new HttpStatusCodeResult(200);
            if (result == FolderServiceResult.AlreadyExist) return new HttpStatusCodeResult(400);
            if (result == FolderServiceResult.InvalidAccessLevel) return new HttpStatusCodeResult(403);
            if (result== FolderServiceResult.MaxFolderDepthReached) return new HttpStatusCodeResult(404);

            return new HttpStatusCodeResult(500);
        }

        [Route("folders/move")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult MoveItem(MoveItemViewModel model)
        { 
            var result= _service.MoveFolder(model);
            if (result== FolderServiceResult.InvalidModelState) return new HttpStatusCodeResult(400);
            if (result== FolderServiceResult.Prohibited) return new HttpStatusCodeResult(405);
            if (result== FolderServiceResult.Success) return new HttpStatusCodeResult(200);

            return new HttpStatusCodeResult(500);
        }

        //POST: /folders/create
        [Route("folders/delete")]
        [HttpPost]
        [ValidateHeaderAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var result = _service.DeleteFolder(id);
            if (result==FolderServiceResult.Success) return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);

            if (result == FolderServiceResult.Prohibited) return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);

            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);  
        }

        //GET: /folders/{id}
        [Route("folders/{folderId}")]
        [HttpGet]
        public ActionResult GetFolder(int folderId)
        {
            GetFolder(out Folder folder, folderId);
            if (folder.Name == GlobalConstants.RootFolderName)
                return RedirectToAction(nameof(Index));
            if (HttpContext.User.IsInRole(RoleNames.Admin))
                ViewBag.AllowCreateFolder = true;
            else
            {
                CheckForUserAccessCode(out bool hasCorrectAccessCode, out double timeSinceLastRequest);
                if (!hasCorrectAccessCode || timeSinceLastRequest > LOCKOUT_TIME)
                    return RedirectToAction(nameof(Index), new { returnUrl = $"/folders/{folderId}" });

                VerifyAccessAndFilterFolderSubFolders(out bool hasAuthorizedAccessToFolder, folder);
                if (!hasAuthorizedAccessToFolder)
                    return RedirectToAction(nameof(Index));

                var userIsnotPermitted = IsCreateActionForbidden(folder);
                if (userIsnotPermitted)
                    ViewBag.AllowCreateFolder = false;
                else
                    ViewBag.AllowCreateFolder = true;
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
            if (result == FolderServiceResult.InvalidModelState) return new HttpStatusCodeResult(400);

            if (result == FolderServiceResult.Success) return new HttpStatusCodeResult(200);

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

            CreateFolderViewModel model;
            if(!HttpContext.User.IsInRole(RoleNames.Admin))
                model = new CreateFolderViewModel
                {
                    Id = id,
                    AccessLevelId = (int)folder.AccessLevelId,
                    AccessLevels = _service.GetCurrentUserAllowedAccessLevels(HttpContext.User.Identity.GetUserId()),
                    Name = folder.Name,
                };
            else
                model = new CreateFolderViewModel
                {
                    Id = id,
                    AccessLevelId = (int)folder.AccessLevelId,
                    AccessLevels = _service.GetAllAccessLevels(),
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
            ViewBag.NewParentFolderId = Request.QueryString["newParentFolderId"];
            return PartialView("_ConfirmItemMove");
        }
         
        //GET: /Folder/VerifyAccessCode
        public ActionResult VerifyAccessCode(string accessCode)
        {
            if (string.IsNullOrWhiteSpace(accessCode))
                return new HttpStatusCodeResult(400);

            var accessCodeInDb = _service.GetCurrentUserAccessCode(HttpContext.User.Identity.GetUserId());
            if (string.IsNullOrWhiteSpace(accessCodeInDb)) 
                return new HttpStatusCodeResult(403);
            var codeIsCorrect=_accessCodeGenerator.VerifyCode(accessCode, accessCodeInDb);
            if (!codeIsCorrect)
                return new HttpStatusCodeResult(400);

            Session[GlobalConstants.IsAccessValidated] = true;
            Session[GlobalConstants.LastVisit] = DateTime.Now;
            return new HttpStatusCodeResult(200);
        }


        #region Private Methods
        private void CheckForUserAccessCode(out bool hasCorrectAccessCode, out double timeSinceLastRequest)
        {
            hasCorrectAccessCode =
               Session[GlobalConstants.IsAccessValidated] != null && (bool)Session[GlobalConstants.IsAccessValidated];
            var lastVisitTime = Session[GlobalConstants.LastVisit] ?? new DateTime();
            timeSinceLastRequest = (DateTime.Now - (DateTime)lastVisitTime).TotalMinutes;
        }


        private void GetFolder(out Folder folder, int folderId)
        {
            folder = _service.GetFolder(folderId);
        }
        private FolderPageViewModel GetModelUsingService(string returnUrl)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var userRoles = _roleService.GetCurrentUserRoles();
            var model = _service.GetRootFolderPageViewModel(userId, userRoles);
            model.ReturnUrl = returnUrl;
            return model;
        }

        private FolderPageViewModel GetRootViewModel(string returnUrl, string search = null)
        {
            if (string.IsNullOrEmpty(search))
                return GetModelUsingService(returnUrl);

            return SearchForFolders(search);
        }
        private FolderPageViewModel GetViewModelForView(Folder folder, int folderId)
        {
            var folderpath = _service.GetFolderPath(folderId);
            var model = Mapper.Map<FolderPageViewModel>(folder);
            model.CloseAccessCodeModal = true;
            model.CurrentPath = folderpath;
            Session[GlobalConstants.LastVisit] = DateTime.Now;
            return model;
        }
        private FolderPageViewModel SearchForFolders(string search)
        {
            var folders = _service.GetFoldersThatMatchName(search).ToList();
            if (!HttpContext.User.IsInRole(RoleNames.Admin))
                folders = FilterFolderSearchResult(folders);
            var model = new FolderPageViewModel
            {
                DirectChildren = folders,
                CurrentPath = new List<FolderPath>(),
                CloseAccessCodeModal = true,
                Id = 0
            };
            return model;
        }

        private List<Folder> FilterFolderSearchResult(List<Folder> folders)
        {
            var userData=_service.GetUserData(HttpContext.User.Identity.GetUserId());
            List<Folder> returnObj= new List<Folder>();
            returnObj.AddRange(folders.Where(f => f.DepartmentId == userData.User.DepartmentId));
            var obj = folders.SingleOrDefault(x => x.FacultyId != null && x.FacultyId == userData.User.Department.FacultyId);
            if(obj != null)
                returnObj.Add(obj);
            if (returnObj.Count() == 0 )
                return null;
            return returnObj;
        }

        private bool IsCreateActionForbidden(Folder folder)
        {
            //parent folder is faculty folder and user is not faculty officer 
            if (folder.FacultyId != null && !HttpContext.User.IsInRole(RoleNames.FacultyOfficer))
                return true;

            //parent folder is departmental folder and user is not departmental officer
            if (folder.DepartmentId != null && folder.IsRestricted && !HttpContext.User.IsInRole(RoleNames.DeptOfficer))
                return true;

            if (folder.Name == GlobalConstants.RootFolderName)
                return true;

            return false;
        }
        private void VerifyAccessAndFilterFolderSubFolders(out bool hasAuthorizedAccessToFolder, Folder folder)
        {
            var userId = HttpContext.User.Identity.GetUserId();
            var userData = _service.GetUserData(userId);
            hasAuthorizedAccessToFolder = _service.DoesUserHasAccessToFolder(folder,userData);
            if (hasAuthorizedAccessToFolder)
                _service.FilterFolderSubFoldersUsingAccessLevel(folder, userData.UserAccessLevel);

            TempData["errorMessage"] =
                hasAuthorizedAccessToFolder ? null : $"You are not authorized to view {folder.Name} folder";

        }

        

       
        #endregion
    }
}

