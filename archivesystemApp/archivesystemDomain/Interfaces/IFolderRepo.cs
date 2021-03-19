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
        void DeleteFolder(int folderId);
        Folder GetFacultyFolder(string name);
        List<FolderPath> GetFolderPath(int id);
        void MoveFolder(int id, int newParentFolderId);
        bool UpdateDepartmentalFolder(Folder model);
        bool UpdateFacultyFolder(Folder model);
        bool UpdateFolder(Folder folder);

    }
}
