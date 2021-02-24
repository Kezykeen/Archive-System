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
        Folder GetFolderByName(string name);
        void UpdateFolder(Folder folder);
        void DeleteFolders(List<Folder> folders);

    }
}
