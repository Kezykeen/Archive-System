using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

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
        public Folder GetRootWithSubfolder()
        {
            var folder = _context.Folders.Include(c=> c.Subfolders).SingleOrDefault(x => x.Name == "Root");
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

        public void DeleteFolder(int folderId)
        {
            RecursiveDelete(folderId);
            _context.Folders.RemoveRange(Folders);
        }

    
        public List<Folder> GetMatchingFolders(string name)
        {
            return _context.Folders.Where(x => x.Name.Contains(name)).ToList();
        }

        public Folder GetFolder(int id)
        {
            var folder = _context.Folders.Include(x=> x.Subfolders).Include(x=>x.Files).Single(x=> x.Id==id);
            return folder;
        }

        public Stack<Folder> GetFolderPath(int id)
        {
            var folders = new Stack<Folder>();
            var isNotRootFolder = true;
            folders.Push(_context.Folders.SingleOrDefault(x => x.Name == "Root"));
            var stack = new Stack<Folder>();
            while (isNotRootFolder)
            {
                var folder = _context.Folders.SingleOrDefault(x => x.Id == id);
                if (folder.ParentId == null)
                    isNotRootFolder = false;
                else
                {
                    stack.Push(folder);
                    id = (int)folder.ParentId;
                }
            }
            while (stack.Count() > 0)
            {
                folders.Push(stack.Pop());
            }

            return folders;
        }

        public void AddFolderPath(int folderId)
        {
            Folder currentFolder = _context.Folders.Find(folderId);
            Folder currentFolderParent = _context.Folders.Find(currentFolder.ParentId);
            currentFolder.Path = currentFolderParent.Path + $",{currentFolder.Id}#{currentFolder.Name}";
            _context.SaveChanges();
        }

       
        private void RecursiveDelete (int folderId)
        {
            var folder =_context.Folders.Include(x=> x.Subfolders).Single(x=> x.Id== folderId);
            var subFolderCount = folder.Subfolders ?? new List<Folder>();
            if(subFolderCount.Count()>0)
            {
                foreach (Folder _folder in folder.Subfolders)
                {
                    RecursiveDelete(_folder.Id);
                }
            }

            if(folder.IsDeletable)
                Folders.Add(folder);
        }
    }
}