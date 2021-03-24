using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderModels;
using System;
using System.Collections.Generic;
using System.Linq;


namespace archivesystemWebUI.Services
{
    public enum FolderServiceResult
    {
        AlreadyExist, InvalidAccessLevel, MaxFolderDepthReached, Success
            , InvalidModelState, Prohibited, NotFound, UnknownError
    }
    public class FolderService : IFolderService
    {
        private IUnitOfWork _repo { get; set; }
        private const int  _GroundLevelAccess=0;

        public FolderService(IUnitOfWork unitofwork)
        {
            _repo = unitofwork;
        }

        public FolderServiceResult DeleteFolder(int folderId)
        {
            try
            {
                var folderToDelete = _repo.FolderRepo.Get(folderId);
                if (folderToDelete == null)
                    return FolderServiceResult.NotFound;
                if (folderToDelete.IsRestricted || folderToDelete.Name== GlobalConstants.RootFolderName)
                    return FolderServiceResult.Prohibited;

                _repo.FolderRepo.DeleteFolder(folderToDelete.Id);
                _repo.Save();
                return FolderServiceResult.Success;
            }
            catch
            {
                return FolderServiceResult.UnknownError;
            }

        }

        public bool DoesUserHasAccessToFolder(Folder folder,UserData data)
        {
            if (data.User == null) return false;
            if (folder.AccessLevelId > data.UserAccessLevel) return false;

            if (folder.FacultyId != null && data.User.Department.FacultyId != folder.FacultyId)//folder is faculty folder
                return false;
            if (folder.DepartmentId != null && folder.DepartmentId != data.User.DepartmentId)
                return false;
            
            return true;
        }

        public FolderServiceResult Edit(CreateFolderViewModel model)
        {
            if (model.AccessLevelId == 0 || string.IsNullOrEmpty(model.Name) || model.Id == 0)
                return FolderServiceResult.InvalidModelState;

            var folderInDb = _repo.FolderRepo.Get(model.Id);
            if (folderInDb == null) return FolderServiceResult.NotFound;
            if (folderInDb.Name == GlobalConstants.RootFolderName) return FolderServiceResult.AlreadyExist;
            if (folderInDb.IsRestricted) return FolderServiceResult.Prohibited;
            

            folderInDb.Name = model.Name;
            folderInDb.AccessLevelId = model.AccessLevelId;
            _repo.Save();
            return FolderServiceResult.Success;
        }

        public Folder FilterFolderSubFoldersUsingAccessLevel(Folder folder,int userAccessLevel)
        {
            var subfolders = folder.Subfolders.Where(x => x.AccessLevelId <= userAccessLevel);
            folder.Subfolders = subfolders.Count() != 0 ? subfolders.ToList(): new List<Folder>();
            return folder;
        }

        public CreateFolderViewModel GetCreateFolderViewModel(int parentId,string userId)
        {
            var parentFolder = _repo.FolderRepo.Get(parentId);
            if (parentFolder == null) return null;

            var user = _repo.UserRepo.FindWithNavProps(c => c.UserId == userId, _ => _.Department).SingleOrDefault();
            if (user == null) return null;

            var userdetails = _repo.AccessDetailsRepo.Find(x => x.AppUserId == user.Id).SingleOrDefault();
            if(userdetails==null) return null;
            
            var userAllowedLevels=_repo.AccessLevelRepo.GetAll().Where(x => x.Id <= userdetails.AccessLevelId);
            userAllowedLevels = userAllowedLevels.Where(x => x.Id >= parentFolder.AccessLevelId);
            var data = new CreateFolderViewModel() { Name = "", ParentId = parentId, AccessLevels = userAllowedLevels };
            return data;
        }

        public IEnumerable<AccessLevel> GetCurrentUserAllowedAccessLevels(string userId)
        {
            var users = _repo.UserRepo.Find(c => c.UserId == userId);
            var user= users.SingleOrDefault();
            if (user == null) return null;

            var userdetails = _repo.AccessDetailsRepo.Find(x => x.AppUserId == user.Id).SingleOrDefault();
            var userAccessLevel = userdetails == null ? 0 : userdetails.AccessLevelId;
            var allowedAccessLevels=_repo.AccessLevelRepo.GetAll().Where(x => x.Id <= userAccessLevel);
            if (allowedAccessLevels.Count() == 0) return null;

            return allowedAccessLevels;
        }

        public string GetCurrentUserAccessCode(string userId)
        {
            var user = _repo.UserRepo.GetUserByUserId(userId);
            if (user == null)
                return "";
            var userDetails = _repo.AccessDetailsRepo.Find(x => x.AppUserId == user.Id).SingleOrDefault();
            if (userDetails == null) return "";
            return userDetails.AccessCode;
        }

        public IEnumerable<Folder> GetFoldersThatMatchName(string name)
        {
            return _repo.FolderRepo.FindWithNavProps(x => x.Name.Contains(name));
        }

        public List<FolderPath> GetFolderPath(int folderId)
        {
            return _repo.FolderRepo.GetFolderPath(folderId);
        }

        public Folder GetFolder(int folderId)
        {
            return _repo.FolderRepo.FindWithNavProps(x => x.Id == folderId, x => x.Subfolders).Single(x => x.Id == folderId);
        }

        public Folder GetRootFolder()
        {
            return _repo.FolderRepo
                    .FindWithNavProps(x => x.Name == "Root" && x.FacultyId == null && x.DepartmentId == null,
                    x => x.Subfolders)
                    .FirstOrDefault();
        }

        public FolderPageViewModel GetRootFolderPageViewModel(string userId,string userRole)
        {
            UserData data = GetUserData(userId);
            if (data.UserAccessLevel == _GroundLevelAccess && !userRole.Contains(RoleNames.Admin))
                return new FolderPageViewModel(); ;
            var rootFolder = GetRootFolder();
            var model = new FolderPageViewModel
            {
                Id = rootFolder.Id,
                Name = rootFolder.Name,
                CurrentPath = new List<FolderPath> { new FolderPath { Id = rootFolder.Id, Name = GlobalConstants.RootFolderName } },
                DirectChildren = userRole == RoleNames.Admin ?
                rootFolder.Subfolders :
                rootFolder.Subfolders.Where(x => x.FacultyId == data.User.Department.FacultyId).ToList()
            };
            return model;
        }

        public UserData GetUserData(string userId)
        {
            var user = _repo.UserRepo.FindWithNavProps(c => c.UserId == userId, _ => _.Department).SingleOrDefault();
            if (user == null) return new UserData { User = null, UserAccessLevel = 0 };
            var userdetails = _repo.AccessDetailsRepo.Find(x=> x.AppUserId==user.Id).SingleOrDefault();
            var userAccessLevel = userdetails == null ? 0 : userdetails.AccessLevelId;
            return  new UserData { User = user, UserAccessLevel = userAccessLevel };
        }

        public FolderServiceResult MoveFolder(MoveItemViewModel model)
        {
            if (model.Id == 0 || model.NewParentFolderId == 0 || model == null)
                return FolderServiceResult.InvalidModelState;
            if (model.Id == model.NewParentFolderId)
                return FolderServiceResult.Prohibited; //Cannot move folder into itself
            
            var moveIsSuccessful=TryMoveFolder(model);
            if(moveIsSuccessful)
                return FolderServiceResult.Success;
            return FolderServiceResult.Prohibited;
        }

        public FolderServiceResult SaveFolder(SaveFolderViewModel model)
        {
            var parentFolder = GetFolder(model.ParentId);
            if (parentFolder.Name == GlobalConstants.RootFolderName)
                return FolderServiceResult.Prohibited;
            if (parentFolder.Subfolders.Select(x => x.Name).Contains(model.Name) || model.Name == GlobalConstants.RootFolderName)
                return FolderServiceResult.AlreadyExist;
            if (parentFolder.AccessLevelId > model.AccessLevelId)
                return FolderServiceResult.InvalidAccessLevel;
            if (GetFolderCurrentDepth(model.ParentId) >= GlobalConstants.MaxFolderDepth)
                return FolderServiceResult.MaxFolderDepthReached;

            TrySaveFolder(model);
            return FolderServiceResult.Success;
        }

        
        #region Private Methods
        private int GetFolderCurrentDepth(int parentId)
        {
            var parentFolderPath = GetFolderPath(parentId);
            return parentFolderPath.Count();
        }

        private bool TryMoveFolder(MoveItemViewModel model)
        {
            try
            {
                _repo.FolderRepo.MoveFolder(model.Id, model.NewParentFolderId);
                _repo.Save();
                return true;
            }
            catch (ArgumentException) { return false; } 
        }
        private void TrySaveFolder(SaveFolderViewModel model)
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
            _repo.FolderRepo.Add(folder);
            _repo.Save();
        }


        #endregion
    }
}