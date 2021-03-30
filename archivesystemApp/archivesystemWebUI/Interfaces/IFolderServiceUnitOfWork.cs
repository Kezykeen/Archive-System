using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderModels;
using archivesystemWebUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemWebUI.Interfaces
{
    public interface IFolderServiceUnitOfWork
    {
        IAccessDetailsRepository AccessDetailsRepo { get;  }
        IFolderRepo FolderRepo { get; }
        IUserRepository UserRepo { get;  }
        IAccessLevelRepository AccessLevelRepo { get;  }
        void Save();
    }


}
