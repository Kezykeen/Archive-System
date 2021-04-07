using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class FolderServiceUnitOfWork: IFolderServiceUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IUserRepository UserRepo { get; }
        public IAccessLevelRepository AccessLevelRepo { get; }
        public IAccessDetailsRepository AccessDetailsRepo { get; }
        public IFolderRepo FolderRepo { get; }
        public IFileRepo FileRepo { get; }



        public FolderServiceUnitOfWork(
            ApplicationDbContext context,
            IUserRepository userRepo,
            IAccessLevelRepository accessLevelRepo,
            IAccessDetailsRepository accessDetailsRepository,
            IFolderRepo folderRepo,
            IFileRepo fileRepo
            )
        {
            _context = context;
            AccessLevelRepo = accessLevelRepo;
            AccessDetailsRepo = accessDetailsRepository;
            UserRepo = userRepo;
            FolderRepo = folderRepo;
            FileRepo = fileRepo;

        }


        public void Save()
        {
            _context.SaveChanges();
            return;
        }

    }
}