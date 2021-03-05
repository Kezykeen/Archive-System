using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using archivesystemDomain.Services;


namespace archivesystemDomain.Interfaces
{
    public interface IFolderRepo : IRepository<Folder>
    {
        Folder GetRootFolder();
        Folder GetFolderByName(string name);
        void UpdateFolder(Folder folder);
        void DeleteFolder(int folderId);
        List<Folder> GetFoldersThatMatchName(string name);
        Folder GetRootWithSubfolder();
        Folder GetFolder(int id);
        List<FolderPath> GetFolderPath(int id);
        void SaveFolderPath(int folderId);
        Folder GetFacultyFolder(string name);
        void MoveFolder(int id, int newParentFolderId);
    }
}
