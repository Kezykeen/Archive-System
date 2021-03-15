using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemWebUI.Interfaces
{
    public interface IFolderService
    {
        void SaveFolder(Folder parentFolder, List<FolderPath> parentFolderPath,
            string folderName, int? accessLevelId);
        bool DoesUserHasAccessToFolder(Folder folder);
        List<FolderPath> GetFolderPath(int folderId);
        void GetUserData(out Employee user, out int userAccessLevel);
        Folder GetRootWithSubfolder();
        IEnumerable<Folder> GetFoldersThatMatchName(string search);
        bool DeleteFolder(int folderId);
    }
}
