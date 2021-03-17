using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models.FolderViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace archivesystemWebUI.Services
{
    public enum FolderActionResult
    {
        AlreadyExist, InvalidAccessLevel , MaxFolderDepthReached , Success
            ,InvalidModelState , Prohibited ,NotFound ,UnknownError
    }
    public class FolderService : IFolderService
    {
        private IUnitOfWork repo { get; set; }
       
        public FolderService(IUnitOfWork unitofwork)
        {
            repo = unitofwork;
        }
       

        public FolderActionResult DeleteFolder(int folderId)
        {
            try
            {
                var folderToDelete = repo.FolderRepo.Get(folderId);
                if (folderToDelete == null)
                    return FolderActionResult.NotFound;
                if (folderToDelete.IsRestricted)
                    return FolderActionResult.Prohibited;

                repo.FolderRepo.DeleteFolder(folderToDelete.Id);
                repo.Save();
                return FolderActionResult.Success;
            }
            catch
            {
                return FolderActionResult.UnknownError;
            }

        }

        public bool DoesUserHasAccessToFolder(Folder folder)
        {
            GetUserData(out AppUser user, out int userAccessLevel);
            SetVariables(out bool isProhibited, out bool hasDepartmentId, folder, userAccessLevel, user);

            if (folder.FacultyId != null && user.Department.FacultyId != folder.FacultyId)
                return false;

            if (isProhibited && hasDepartmentId)
                return false;

            return true;


        }

        public FolderActionResult Edit(CreateFolderViewModel model)
        {
            if (model.AccessLevelId == 0 || string.IsNullOrEmpty(model.Name) || model.Id == 0)
                return FolderActionResult.InvalidModelState;
            var folder = new Folder { Name = model.Name, Id = model.Id, AccessLevelId = model.AccessLevelId };
            repo.FolderRepo.UpdateFolder(folder);
            repo.Save();

            return FolderActionResult.Success;
        }

        public CreateFolderViewModel GetCreateFolderViewModel(int parentId)
        {
            var parentFolder = repo.FolderRepo.Get(parentId);
            var userAllowedLevels = GetCurrentUserAllowedAccessLevels();
            userAllowedLevels = userAllowedLevels.Where(x => x.Id >= parentFolder.AccessLevelId);
            var data = new CreateFolderViewModel() { Name = "", ParentId = parentId, AccessLevels =userAllowedLevels  };
            return data;
        }

        public IEnumerable<AccessLevel> GetCurrentUserAllowedAccessLevels()
        {
            GetUserData(out AppUser user, out int userAccessLevel);
            return repo.AccessLevelRepo.GetAll().Where(x => x.Id <= userAccessLevel);
        }
        public string GetCurrentUserAccessCode()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            var user = repo.UserRepo.GetUserByMail(userId);
            var userAccessCode = repo.AccessDetailsRepo.GetByEmployeeId(user.Id).AccessCode;
            return userAccessCode;
        }

        public IEnumerable<Folder> GetFoldersThatMatchName(string name)
        {
            return repo.FolderRepo.FindWithNavProps(x => x.Name.Contains(name));
        }

        public List<FolderPath> GetFolderPath(int folderId)
        {
            return repo.FolderRepo.GetFolderPath(folderId);
        }

        public Folder GetFolder(int folderId)
        {
            return repo.FolderRepo.FindWithNavProps(x => x.Id == folderId, x => x.Subfolders).Single(x => x.Id == folderId);
        }

        public Folder GetRootFolder()
        {
            return  repo.FolderRepo
                    .FindWithNavProps(x => x.Name == "Root" && x.FacultyId == null && x.DepartmentId == null,
                    x=>x.Subfolders)
                    .FirstOrDefault();
        }
        public void GetUserData(out AppUser user, out int userAccessLevel)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            user = repo.UserRepo.GetUserByUserId(userId);
            userAccessLevel = repo.AccessDetailsRepo.GetByEmployeeId(user.Id).AccessLevelId;
        }

        public FolderActionResult MoveFolder(MoveItemViewModel model)
        {
            if (model.Id == 0 || model.NewParentFolderId == 0 || model==null)
                return FolderActionResult.InvalidModelState;
            if (model.Id == model.NewParentFolderId)
                return FolderActionResult.Prohibited; //Cannot move folder into itself
            repo.FolderRepo.MoveFolder(model.Id, model.NewParentFolderId);
            repo.Save();
            return FolderActionResult.Success;
        }

        public FolderActionResult TrySaveFolder(SaveFolderViewModel model)
        {
            var parentFolder = GetFolder(model.ParentId);
            if (parentFolder.Name == "Root")
                return FolderActionResult.Prohibited;
            if (parentFolder.Subfolders.Select(x => x.Name).Contains(model.Name) || model.Name == "Root")
                return FolderActionResult.AlreadyExist;
            if (parentFolder.AccessLevelId > model.AccessLevelId)
                return FolderActionResult.InvalidAccessLevel;
            if (GetFolderCurrentDepth(model.ParentId) >= (int)AllowableFolderDepth.Max)
                return FolderActionResult.MaxFolderDepthReached;

            SaveFolder(model);
            return FolderActionResult.Success;
        }

        #region Private Methods
        private int GetFolderCurrentDepth(int parentId)
        {
            var parentFolderPath = GetFolderPath(parentId);
            return parentFolderPath.Count();
        }
        public void SaveFolder(SaveFolderViewModel model)
        {
            var parentFolder = GetFolder(model.ParentId);
            var folder = new Folder
            {
                Name = model.Name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ParentId = parentFolder.Id,
                AccessLevelId = model.AccessLevelId,
                IsRestricted = false,
                DepartmentId = parentFolder.DepartmentId,
            };
            repo.FolderRepo.Add(folder);
            repo.Save();
        }

        private void SetVariables(out bool isProhibited, out bool hasDepartmentId, Folder folder,
            int userAccessLevel, AppUser user)
        {
            isProhibited = folder.AccessLevelId > userAccessLevel || folder.DepartmentId != user.DepartmentId;
            hasDepartmentId = folder.DepartmentId != null;
            folder.Subfolders = folder.Subfolders.Where(x => x.AccessLevelId <= userAccessLevel).ToList();
        }

        #endregion
    }
}