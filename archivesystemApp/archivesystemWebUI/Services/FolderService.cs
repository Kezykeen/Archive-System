using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Services
{
    public class FolderService : IFolderService
    {

        private IUnitOfWork repo { get; set; }
       
        public FolderService(IUnitOfWork unitofwork)
        {
            repo = unitofwork;
        }

        public void SaveFolder(Folder parentFolder, List<FolderPath> parentFolderPath, string folderName, int? accessLevelId)
        {
            var folder = new Folder
            {
                Name = folderName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                ParentId = parentFolder.Id,
                AccessLevelId = accessLevelId,
                IsRestricted = false,
                DepartmentId = parentFolder.DepartmentId,
            };
            repo.FolderRepo.Add(folder);
            repo.Save();
        }

        public bool DoesUserHasAccessToFolder(Folder folder)
        {
            GetUserData(out Employee user, out int userAccessLevel);
            SetVariables(out bool isProhibited, out bool hasDepartmentId, folder, userAccessLevel, user);

            if (folder.FacultyId != null && user.Department.FacultyId != folder.FacultyId)
                return false;

            if (isProhibited && hasDepartmentId)
                return false;

            return true;


        }

        public List<FolderPath> GetFolderPath(int folderId)
        {
            return repo.FolderRepo.GetFolderPath(folderId);
        }

        public void GetUserData(out Employee user, out int userAccessLevel)
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            user = repo.EmployeeRepo.GetEmployeeByUserId(userId);
            userAccessLevel = repo.AccessDetailsRepo.GetByEmployeeId(user.Id).AccessLevelId;
        }

        public bool DeleteFolder(int folderId)
        {
            try
            {
                var folderToDelete = repo.FolderRepo.Get(folderId);
                if (folderToDelete.IsRestricted)
                    return false;

                repo.FolderRepo.DeleteFolder(folderToDelete.Id);
                repo.Save();
                return true;
            }
            catch
            {
                return false;
            }

        }

        public Folder GetRootWithSubfolder()
        {
            return repo.FolderRepo.GetRootWithSubfolder();
        }
        public IEnumerable<Folder> GetFoldersThatMatchName(string search)
        {
            return repo.FolderRepo.GetFoldersThatMatchName(search);
        }


        private void SetVariables(out bool isProhibited, out bool hasDepartmentId, Folder folder,
            int userAccessLevel, Employee user)
        {
            isProhibited = folder.AccessLevelId > userAccessLevel || folder.DepartmentId != user.DepartmentId;
            hasDepartmentId = folder.DepartmentId != null;
            folder.Subfolders = folder.Subfolders.Where(x => x.AccessLevelId <= userAccessLevel).ToList();
        }
    }
}