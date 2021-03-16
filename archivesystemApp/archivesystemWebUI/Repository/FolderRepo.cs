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
        private List<Folder> Folders = new List<Folder>();
        private List<FolderPath> CurrentPathFolders = new List<FolderPath>();

        public FolderRepo(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public void DeleteFolder(int folderId)
        {
            RecursiveDelete(folderId);
            _context.Folders.RemoveRange(Folders);
        }
        public Folder GetFacultyFolder(string name)
        {
            var rootFolder = Find(x=> x.Name=="Root" && x.FacultyId==null && x.DepartmentId==null).FirstOrDefault();
            var folder = _context.Folders.Include(x=> x.Subfolders).SingleOrDefault(x => x.Name == name && x.ParentId == rootFolder.Id);
            return folder;
        }
        public List<FolderPath> GetFolderPath(int folderId)
        {
            var currentfolder = _context.Folders.Find(folderId);
            if (currentfolder.Name == "Root")
            {
                CurrentPathFolders.Add(new FolderPath { Name = "Root", Id = currentfolder.Id });
                return CurrentPathFolders;
            }
            else
            {
                CurrentPathFolders.Add(new FolderPath { Id = currentfolder.Id, Name = currentfolder.Name });
                GetFolderPath((int)currentfolder.ParentId);
            }

            return CurrentPathFolders;
        }
        public void MoveFolder(int id, int newParentFolderId)
        {
            var folder = _context.Folders.Find(id);
            if (folder.IsRestricted || folder == null)
                return;
            var currentSubfolderNames = _context.Folders.Include(x => x.Subfolders).Single(x => x.Id == newParentFolderId)
                .Subfolders.Select(x => x.Name);
            if (currentSubfolderNames.Contains(folder.Name))
                throw new Exception("folder already exist in location");

            folder.ParentId = newParentFolderId;
            return;
        }
        private void RecursiveDelete(int folderId)
        {
            var folder = _context.Folders.Include(x => x.Subfolders).Single(x => x.Id == folderId);
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

            Folders.Add(folder);
        }
        public void UpdateFolder(Folder folder)
        {
            var folderInDb = _context.Folders.Find(folder.Id);
            if (folderInDb == null || folderInDb.IsRestricted)
                return;
            folderInDb.Name = folder.Name;
            folderInDb.AccessLevelId = folder.AccessLevelId;
            folder.UpdatedAt = DateTime.Now;
        }
        public void UpdateDepartmentalFolder(Folder model)
        {
            var folderInDb = _context.Folders.Include(x=> x.Department).SingleOrDefault(x => x.IsRestricted && x.DepartmentId == model.DepartmentId);
            folderInDb.Name = model.Name;
            if(folderInDb.Department.FacultyId != model.FacultyId)
            {
                var newParentFolder = _context.Folders.SingleOrDefault(x => x.IsRestricted && x.FacultyId == model.FacultyId);
                folderInDb.ParentId = newParentFolder.Id;
            }
            
        }
        public void UpdateFacultyFolder(Folder model)
        {
            var folderInDb = _context.Folders.Include(x => x.Department).SingleOrDefault(x => x.IsRestricted && x.FacultyId == model.FacultyId);
            folderInDb.Name = model.Name;
        }

        

        

        
    }
}