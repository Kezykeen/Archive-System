using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;
using Microsoft.AspNet.Identity.Owin;

namespace archivesystemDomain.Services
{
    public static class SeedAppData
    {
       
       
        public static void EnsurePopulated()
        {
            //var dbContext = new ApplicationDbContext();
            //var rootFolder = dbContext.Folders.SingleOrDefault(x => x.Name == "Root");
            ////var baseAccess = dbContext.AccessLevels.SingleOrDefault(x => x.Level == AccessLevelNames.BaseLevel);

            //if (!dbContext.Faculties.Any())
            //{
            //    //if (baseAccess == null)
            //    //{
            //    //    //Seed base access level
            //    //    baseAccess = new AccessLevel
            //    //    {
            //    //        Level = AccessLevelNames.BaseLevel,
            //    //        LevelDescription = "Base level",
            //    //        CreatedAt = DateTime.Now,
            //    //        UpdatedAt = DateTime.Now
            //    //    };
            //    //    dbContext.AccessLevels.Add(baseAccess);
            //    //}

            //    //Seed faculties
            //    List<Faculty> faculties = new List<Faculty>
            //    {
            //        new Faculty
            //        {
            //            Name = "Engineering",
            //            CreatedAt = DateTime.Now,
            //            UpdatedAt = DateTime.Now
            //        },
            //        new Faculty
            //        {
            //            Name = "Arts",
            //            CreatedAt = DateTime.Now,
            //            UpdatedAt = DateTime.Now
            //        },
            //    };
            //    dbContext.Faculties.AddRange(faculties);
              

            //    //Seed faculty folders
            //    var facultyFolders = new List<Folder>
            //        {
            //            new Folder
            //            {
            //                Name=faculties[0].Name,
            //                CreatedAt=DateTime.Now,
            //                UpdatedAt=DateTime.Now
            //            },
            //            new Folder
            //            {
            //                Name=faculties[1
            //                ].Name,
            //                CreatedAt=DateTime.Now,
            //                UpdatedAt=DateTime.Now
            //            },
            //        };
            //    dbContext.Folders.AddRange(facultyFolders);
              

            //    ////Seed faculty folders as root subfolders
            //    //dbContext.SubFolders.Add(new SubFolder
            //    //{
            //    //    FolderId = facultyFolders[0].Id,
            //    //    ParentId = rootFolder.Id,
            //    //    AccessLevelId = baseAccess.Id
            //    //});
            //    //dbContext.SubFolders.Add(new SubFolder
            //    //{
            //    //    FolderId = facultyFolders[1].Id,
            //    //    ParentId = rootFolder.Id,
            //    //    AccessLevelId = baseAccess.Id
            //    //});
              
            //    //Seed departments
            //    var departments = new List<Department>()
            //        {
            //            new Department
            //            {
            //                Name = "Electronic Engineering",
            //                CreatedAt = DateTime.Now,
            //                UpdatedAt = DateTime.Now,
            //                FacultyId=faculties[0].Id
            //            },

            //            new Department
            //            {
            //                Name = "People and Culture",
            //                FacultyId=faculties[1].Id,
            //                CreatedAt = DateTime.Now,
            //                UpdatedAt = DateTime.Now
            //            },
            //        };
            //    dbContext.Departments.AddRange(departments);
              
            //    ////Seed department folders
            //    //var departmentFolders = new List<Folder>
            //    //    {
            //    //        new Folder
            //    //        {
            //    //            Name=departments[0].Name,
            //    //            CreatedAt=DateTime.Now,
            //    //            UpdatedAt=DateTime.Now
            //    //        },
            //    //        new Folder
            //    //        {
            //    //            Name=departments[1].Name,
            //    //            CreatedAt=DateTime.Now,
            //    //            UpdatedAt=DateTime.Now
            //    //        },
            //    //    };
            //    //dbContext.Folders.AddRange(departmentFolders);
            


            //    ////Seed department folders as faculty sub folder
            //    //dbContext.SubFolders.Add(new SubFolder
            //    //{
            //    //    FolderId = departmentFolders[0].Id,
            //    //    ParentId = facultyFolders[0].Id,
            //    //    AccessLevelId = baseAccess.Id
            //    //});
            //    //dbContext.SubFolders.Add(new SubFolder
            //    //{
            //    //    FolderId = departmentFolders[1].Id,
            //    //    ParentId = facultyFolders[1].Id,
            //    //    AccessLevelId = baseAccess.Id
            //    //});
            //    dbContext.SaveChanges();
            //}

           
        }
       

    }
}