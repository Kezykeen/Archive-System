using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Repository
{
    public class SubFolderRepo:Repository<SubFolder>, ISubFolderRepo
    {
        private readonly ApplicationDbContext _context;
        private List<Folder> SubFolders = new List<Folder>();

        public SubFolderRepo(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public List<Folder> RecursiveGetSubFolders(Folder folder)
        {
            GetSubFolders(folder);
            return SubFolders;
        }

        private void GetSubFolders(Folder folder)
        {
            var subFolders = _context.SubFolders.Include("Folder").Where(x => x.ParentId == folder.Id).Select(x => x.Folder).ToList();
            foreach (Folder _folder in subFolders)
            {
                RecursiveGetSubFolders(_folder);
            }
            SubFolders.Add(folder);
        }

        public IEnumerable<Folder> GetSubFolders(int rootFolderId)
        {
            IQueryable<Folder> subFolders = _context.SubFolders.Include("Folder").Where(x => x.ParentId == rootFolderId).Select(x => x.Folder);
            return subFolders;
        }

        public void AddToParentFolder(int parentId, int folderId)
        {
            _context.SubFolders.Add(new SubFolder { ParentId = parentId, FolderId = folderId });

        }

        public int GetParentId(int folderId)
        {
            var subFolder = _context.SubFolders.SingleOrDefault(x => x.FolderId == folderId);
            return subFolder.ParentId;

        }

        public IEnumerable<string> GetSubFolderNames(int folderId)
        {
            return _context.SubFolders.Include("Folder").Where(x => x.ParentId == folderId).Select(x => x.Folder.Name).ToList();
        }

        public SubFolder GetByFolderId(int folderId)
        {
            return _context.SubFolders.Include("Folder").SingleOrDefault(x => x.FolderId == folderId);
        }

        public void  Update(SubFolder subFolder)
        {
            var subFolderInDb = _context.SubFolders.SingleOrDefault(x=> x.FolderId== subFolder.FolderId);
            if(subFolderInDb !=null)
                subFolderInDb.AccessLevelId = subFolder.AccessLevelId;
            return;
        }

        public Stack<Folder> GetFolderPath(int  id)
        {
            var folders = new Stack<Folder>();
            var isNotRootFolder = true;
            folders.Push(_context.Folders.SingleOrDefault(x => x.Name == "Root"));
            var stack = new Stack<Folder>();
            while (isNotRootFolder)
            {
                var subFolder = _context.SubFolders.Include("Folder").SingleOrDefault(x => x.FolderId == id);
                if (subFolder ==null)
                    isNotRootFolder = false;
                else
                {
                    stack.Push(subFolder.Folder);
                    id = subFolder.ParentId;
                }
            }
            while (stack.Count() > 0)
            {
                folders.Push(stack.Pop());
            }
            
            return folders;
        }
    }
}