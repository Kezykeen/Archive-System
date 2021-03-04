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
             var dbContext = new ApplicationDbContext();
             var baseAccess = dbContext.AccessLevels.SingleOrDefault(x => x.Level == AccessLevelNames.BaseLevel);
            
             if (!dbContext.Faculties.Any())
             {
                 if (baseAccess == null)
                 {
                     //Seed base access level
                     baseAccess = new AccessLevel
                     {
                         Level = AccessLevelNames.BaseLevel,
                         LevelDescription = "Base level",
                         CreatedAt = DateTime.Now,
                         UpdatedAt = DateTime.Now
                     };
                     dbContext.AccessLevels.Add(baseAccess);
                   
                 }
                 // Seed faculties
                 List<Faculty> faculties = new List<Faculty>
                 {
                     new Faculty
                     {
                         Name = "Engineering",
                         CreatedAt = DateTime.Now,
                         UpdatedAt = DateTime.Now,
                         Departments= new List<Department>()
                         {
                             new Department
                             {
                                 Name = "Electronic Engineering",
                                 CreatedAt = DateTime.Now,
                                 UpdatedAt = DateTime.Now
                             }
                         }
                        
                     },
                     new Faculty
                     {
                         Name = "Arts",
                         CreatedAt = DateTime.Now,
                         UpdatedAt = DateTime.Now,
                         Departments = new List<Department>
                         {
                             new Department
                             {
                                 Name = "People and Culture",
                                 CreatedAt = DateTime.Now,
                                 UpdatedAt = DateTime.Now
                             }
                         }
                     },
                 };
                 dbContext.Faculties.AddRange(faculties);

                 //Seed folders

                
                 var facultyFolders = new List<Folder>
                     {
                     };

                 faculties.ForEach(f => facultyFolders.Add(
                     new Folder
                     {
                         Name = f.Name,
                         CreatedAt = DateTime.Now,
                         UpdatedAt = DateTime.Now,
                         AccessLevelId = baseAccess.Id,
                         Subfolders = new List<Folder>()

                     }));

                foreach (var folder in facultyFolders)
                {
                    foreach (var faculty in faculties)
                    {
                        if (folder.Name == faculty.Name)
                        {
                            faculty.Departments.ForEach(

                                d => folder.Subfolders.Add(
                                    new Folder
                                    {
                                        Name = d.Name,
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                        AccessLevelId = baseAccess.Id,
                                        ParentId = folder.Id

                                    }

                                )
                            );

                        }
                    }
                   
                }
                 
                 dbContext.Folders.AddRange(facultyFolders);
                //save changes
                dbContext.SaveChanges();
             }


        }
       

    }
}