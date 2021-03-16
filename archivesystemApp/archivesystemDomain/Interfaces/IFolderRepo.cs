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
        void UpdateFolder(Folder folder);
        void DeleteFolder(int folderId);
        List<FolderPath> GetFolderPath(int id);
        void UpdateDepartmentalFolder(Folder model);
        Folder GetFacultyFolder(string name);
        void MoveFolder(int id, int newParentFolderId);
        void UpdateFacultyFolder(Folder model);
    }
}
