using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using archivesystemWebUI.Models.FolderViewModels;
using archivesystemWebUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemWebUI.Interfaces
{
    public interface IFolderService
    {
        FolderServiceResult DeleteFolder(int folderId);
        bool DoesUserHasAccessToFolder(Folder folder);
        FolderServiceResult Edit(CreateFolderViewModel model);
        CreateFolderViewModel GetCreateFolderViewModel(int parentId);
        string GetCurrentUserAccessCode();
        IEnumerable<AccessLevel> GetCurrentUserAllowedAccessLevels();
        List<FolderPath> GetFolderPath(int folderId);
        IEnumerable<Folder> GetFoldersThatMatchName(string search);
        Folder GetFolder(int folderId);
        Folder GetRootFolder();
        void GetUserData(out AppUser user, out int userAccessLevel);
        FolderServiceResult MoveFolder(MoveItemViewModel model);
        FolderServiceResult SaveFolder(SaveFolderViewModel model);




    }

    
}
