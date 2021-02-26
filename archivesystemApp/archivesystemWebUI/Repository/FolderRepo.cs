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

        public void UpdateFolder(Folder folder)
        {
            var folderInDb = _context.Folders.Find(folder.Id);
            if (folderInDb == null)
                return;
            folderInDb.Name = folder.Name;
            folder.UpdatedAt = DateTime.Now;
        }

        public void DeleteFolders(List<Folder> folders)
        {
            _context.Folders.RemoveRange(folders);
        }

    
        public List<Folder> GetMatchingFolders(string name)
        {
            return _context.Folders.Where(x => x.Name.Contains(name)).ToList();
        }
    
    }
}