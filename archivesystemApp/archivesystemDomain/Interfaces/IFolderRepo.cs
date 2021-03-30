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
        List<File> GetFilesThatMactchFileName(int folderId, string filename, bool returnall=false);

        /// <summary>
        /// Moves folder from one parent folder to another.
        /// 
        /// Exceptions:
        /// AlreadyExistInLocationException
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newParentFolderId"></param>
        void MoveFolder(int id, int newParentFolderId);
        bool UpdateDepartmentalFolder(Folder model);
        bool UpdateFacultyFolder(Folder model);

    }
}
