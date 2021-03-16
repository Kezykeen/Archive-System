﻿using archivesystemDomain.Entities;
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
        FolderActionResult DeleteFolder(int folderId);
        bool DoesUserHasAccessToFolder(Folder folder);
        FolderActionResult Edit(CreateFolderViewModel model);
        CreateFolderViewModel GetCreateFolderViewModel(int parentId);
        string GetCurrentUserAccessCode();
        IEnumerable<AccessLevel> GetCurrentUserAllowedAccessLevels();
        List<FolderPath> GetFolderPath(int folderId);
        IEnumerable<Folder> GetFoldersThatMatchName(string search);
        Folder GetFolder(int folderId);
        Folder GetRootFolder();
        void GetUserData(out Employee user, out int userAccessLevel);
        FolderActionResult MoveFolder(MoveItemViewModel model);
        FolderActionResult TrySaveFolder(SaveFolderViewModel model);


    }
}
