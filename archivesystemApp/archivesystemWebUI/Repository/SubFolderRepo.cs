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

            return _context.SubFolders.SingleOrDefault(x => x.FolderId == folderId).ParentId;

        }

        public IEnumerable<string> GetSubFolderNames(int folderId)
        {
            return _context.SubFolders.Include("Folder").Where(x => x.ParentId == folderId).Select(x => x.Folder.Name).ToList();
        }

        public SubFolder GetByFolderId(int folderId)
        {
            return _context.SubFolders.Include("Folder").SingleOrDefault(x => x.FolderId == folderId);
        }

        public void Update(SubFolder subFolder)
        {
            var subFolderInDb = _context.SubFolders.SingleOrDefault(x=> x.FolderId== subFolder.FolderId);
            if(subFolderInDb !=null)
                subFolderInDb.AccessLevelId = subFolder.AccessLevelId;
            return;
        }
    }
}