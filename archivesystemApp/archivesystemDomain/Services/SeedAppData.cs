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

            if (!dbContext.Faculties.Any())
            {

                // Seed faculties
                List<Faculty> faculties = new List<Faculty>
                {
                    new Faculty
                    {
                        Name = "Engineering",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        Departments = new List<Department>()
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

                if (!dbContext.Folders.Any())
                {

                    //Seed Root folder

                    var rootFolder = new Folder
                    {
                        Name = "Root",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsDeletable = false,
                        AccessLevel = new AccessLevel
                        {
                            Level = AccessLevelNames.BaseLevel,
                            LevelDescription = "Base level",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        Subfolders = new List<Folder>(),
                        Path = "1#Root"

                    };

                    dbContext.Folders.Add(rootFolder);

                    // rootFolder.Path = $"{rootFolder.Id}#{rootFolder.Name}";

                    var facultyFolders = rootFolder.Subfolders;
                    var facultyFolderId = 0;
                    foreach (var faculty in faculties)
                    {
                        facultyFolderId++;
                        facultyFolders.Add(

                            new Folder
                            {
                                Name = faculty.Name,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                AccessLevelId = 1,
                                Subfolders = new List<Folder>(),
                                ParentId = 1,

                            });
                    }
                });
                dbContext.SaveChanges();

                //Add root folder path
                var rootFolder = dbContext.Folders.SingleOrDefault(x => x.Name == "Root");
                dbContext.SaveChanges();
               
            }


                            }
                        }

                    }
                }
            }

            // save changes
            dbContext.SaveChanges();
        }
    }
    }

        
             
          

           


        
    
       

    
