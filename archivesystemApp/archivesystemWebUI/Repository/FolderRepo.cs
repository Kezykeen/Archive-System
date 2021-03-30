using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using archivesystemDomain.Services;

namespace archivesystemWebUI.Repository
{
    public class FolderRepo : Repository<Folder>, IFolderRepo
    {
        private readonly ApplicationDbContext _context;
        private List<Folder> __folders = new List<Folder>();
        private List<FolderPath> CurrentPathFolders = new List<FolderPath>();

        public FolderRepo(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public void DeleteFolder(int folderId)
        {
            RecursiveDelete(folderId);
            _context.Folders.RemoveRange(__folders);
        }
        public Folder GetFacultyFolder(string name)
        {
            var rootFolder = Find(x=> x.Name== GlobalConstants.RootFolderName && x.FacultyId==null && x.DepartmentId==null)
                .FirstOrDefault();
            var folders = FindWithNavProps(x => x.Name == name , x => x.Subfolders).ToList();
            var folder=folders.SingleOrDefault(x => x.Name == name && x.ParentId == rootFolder.Id);
            return folder;
        }
        public List<FolderPath> GetFolderPath(int folderId)
        {
            var currentfolder = _context.Folders.Find(folderId);
            if (currentfolder.Name == GlobalConstants.RootFolderName)
            {
                CurrentPathFolders.Add(new FolderPath { Name = GlobalConstants.RootFolderName, Id = currentfolder.Id });
                return CurrentPathFolders;
            }
            else
            {
                CurrentPathFolders.Add(new FolderPath { Id = currentfolder.Id, Name = currentfolder.Name });
                GetFolderPath((int)currentfolder.ParentId);
            }

            return CurrentPathFolders;
        }

        public List<File> GetFilesThatMactchFileName(int folderId,string filename,bool returnall=false)
        {
            var data=_context.Folders.Include(x => x.Files.Select( y=> y.FileMeta)).Where(c=> c.Id== folderId);
            if (data == null) return null;

            var folder = data.SingleOrDefault(c => c.Id == folderId);
            if (folder == null) return null;

            if (folder.Files == null) return null;

            if (returnall) return folder.Files.ToList();
            return folder.Files.Where(x=> x.FileMeta.Title.ToLower().Contains(filename.ToLower())).ToList();

        }
        public void MoveFolder(int id, int newParentFolderId)
        {
            var folder = _context.Folders.Find(id);
            if (folder.IsRestricted || folder == null)
                return;
            var currentSubfolderNames =  FindWithNavProps(x=>x.Id == newParentFolderId,x => x.Subfolders)
                                        .FirstOrDefault()
                                        .Subfolders.Select(x => x.Name);
            if (currentSubfolderNames.Contains(folder.Name))
                throw new ArgumentException();

            folder.ParentId = newParentFolderId;
            return;
        }
        private void RecursiveDelete(int folderId)
        {
            var folder = FindWithNavProps(x => x.Id == folderId,x => x.Subfolders).First();
            if (folder.IsRestricted || folder == null)
                return;
            var subFolderCount = folder.Subfolders ?? new List<Folder>();
            if (subFolderCount.Count() > 0)
            {
                foreach (Folder _folder in folder.Subfolders)
                {
                    RecursiveDelete(_folder.Id);
                }
            }

            __folders.Add(folder);
        }
        public bool UpdateDepartmentalFolder(Folder model)
        {
            var folders = FindWithNavProps(x => x.IsRestricted && x.DepartmentId == model.DepartmentId,x=> x.Department)?.ToList();
            if (folders == null || folders.Count()!=1 ) return false;

            var folderInDb=folders.SingleOrDefault();
            if (folderInDb == null) return false;

            folderInDb.Name = model.Name;
            if(folderInDb.Department.FacultyId != model.FacultyId)
            {
                var newParentFolder = _context.Folders.SingleOrDefault(x => x.IsRestricted && x.FacultyId == model.FacultyId);
                folderInDb.ParentId = newParentFolder.Id;
            }
            return true;
        }
        public bool UpdateFacultyFolder(Folder model)
        {
            var folderInDb = FindWithNavProps(x => x.IsRestricted && x.FacultyId == model.FacultyId,x => x.Department)
                            .SingleOrDefault();
            if (folderInDb == null) return false;
            folderInDb.Name = model.Name;
            return true;
        }
    }
}