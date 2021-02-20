using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Interfaces
{
    public interface IFolderRepo : IRepository<Folder>
    {
        Folder GetRootFolder();
        IEnumerable<Folder> GetSubFolders(int rootFolderId);
        void AddToParentFolder(int parentId, int folderId);
        void RecursiveDelete(Folder folder);
        int GetParentId(int folderId);

        IEnumerable<string> GetSubFolderNames(int folderId);

    }
}
