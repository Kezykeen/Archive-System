using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class FolderRepo : Repository<Folder>, IFolderRepo
    {
        private readonly ApplicationDbContext _context; 
        private List<Folder> Folders = new List<Folder>();

        public FolderRepo(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Folder GetRootFolder()
        {
            var folder = _context.Folders.SingleOrDefault(x => x.Name=="Root");
            return folder;
        }  

        public Folder GetFolderByName(string name)
        {
            return _context.Folders.SingleOrDefault(x => x.Name == name);
        }

    }
}