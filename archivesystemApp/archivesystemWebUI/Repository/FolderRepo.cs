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

        public IEnumerable<Folder> GetSubFolders(int rootFolderId)
        {
            IQueryable<Folder> subFolders=_context.SubFolders.Include("Folder").Where(x => x.ParentId == rootFolderId).Select(x => x.Folder);
            return subFolders;
        }

        public void RecursiveDelete(Folder folder)
        {
            RecursiveGetSubFolders(folder);
            _context.Folders.RemoveRange(Folders);
        }

        public void  RecursiveGetSubFolders (Folder folder)
        {
            var subFolders = _context.SubFolders.Include("Folder").Where(x => x.ParentId == folder.Id).Select(x=> x.Folder).ToList();
            foreach(Folder _folder in subFolders)
            {
                RecursiveGetSubFolders(_folder);
            }
            Folders.Add(folder);
        }

        public void AddToParentFolder(int parentId,int folderId)
        {
            _context.SubFolders.Add(new SubFolder { ParentId = parentId, FolderId =folderId});

        }

        public int GetParentId(int folderId)
        {

            return _context.SubFolders.SingleOrDefault(x => x.FolderId == folderId).ParentId;
           
        }

        public IEnumerable<string> GetSubFolderNames(int folderId)
        {
           return  _context.SubFolders.Include("Folder").Where(x => x.ParentId == folderId).Select(x => x.Folder.Name).ToList();
        }

   

    }
}