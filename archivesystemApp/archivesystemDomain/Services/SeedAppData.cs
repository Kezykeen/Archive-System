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
            var rootFolder = dbContext.Folders.SingleOrDefault(x => x.Name == "Root");
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
                    dbContext.SaveChanges();
                }

                //Seed faculties
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
                dbContext.SaveChanges();

                //Seed folders
                var facultyFolders = new List<Folder>
                    {
                        new Folder
                        {
                            Name=faculties[0].Name,
                            CreatedAt=DateTime.Now,
                            UpdatedAt=DateTime.Now,
                            ParentId=rootFolder.Id,
                            AccessLevelId=baseAccess.Id,
                            Subfolders= new List<Folder>
                            {
                                new Folder
                                {
                                    Name=faculties[0].Departments[0].Name,
                                    CreatedAt=DateTime.Now,
                                    UpdatedAt=DateTime.Now,
                                    AccessLevelId=baseAccess.Id,
                                },
                            }

                        },
                        new Folder
                        {
                            Name=faculties[1].Name,
                            CreatedAt=DateTime.Now,
                            UpdatedAt=DateTime.Now,
                            ParentId=rootFolder.Id,
                            AccessLevelId=baseAccess.Id,
                            Subfolders= new List<Folder>
                            {
                                new Folder
                                {
                                    Name=faculties[1].Departments[0].Name,
                                    CreatedAt=DateTime.Now,
                                    UpdatedAt=DateTime.Now,
                                    AccessLevelId=baseAccess.Id,
                                },
                            }
                        },
                    };
                dbContext.Folders.AddRange(facultyFolders);
               
                //save changes
                dbContext.SaveChanges();
            }


        }
       

    }
}