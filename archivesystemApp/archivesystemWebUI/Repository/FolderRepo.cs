using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class FolderRepo: Repository<Folder>,IFolderRepo
    {
        private readonly ApplicationDbContext _context;

        public FolderRepo(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public IEnumerable<Folder> GetAllEager()
        {
            return _context.Folders.Include("Department").ToList();
          
        }
    }
}