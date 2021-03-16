using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepo { get; }
        IDeptRepository DeptRepo { get; }
        IAccessLevelRepository AccessLevelRepo { get; }
        ITokenRepo TokenRepo { get; }
        IFacultyRepository FacultyRepo { get; }
        IAccessDetailsRepository AccessDetailsRepo { get; }
        IFolderRepo FolderRepo { get; }
        IFileMetaRepo FileMetaRepo { get; }
        IFileRepo FileRepo { get; }

        int Save();
        Task<int> SaveAsync();

    }
}