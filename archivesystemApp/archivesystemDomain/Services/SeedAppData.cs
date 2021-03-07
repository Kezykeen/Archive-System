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


                    var facultyFolders = new List<Folder>();
                    foreach (var faculty in faculties)
                    {
                        facultyFolders.Add(

                            new Folder
                            {
                                Name = faculty.Name,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now,
                                AccessLevelId = 1,
                                IsRestricted = true,
                                Subfolders = new List<Folder>
                                { 
                                    new Folder
                                    {
                                        Name=faculty.Departments[0].Name,
                                        CreatedAt = DateTime.Now,
                                        UpdatedAt = DateTime.Now,
                                        AccessLevelId = 1,
                                        IsRestricted=true,
                                    }
                                }                           
                            });
                    }


                    //Seed Root folder
                    var rootFolder = new Folder
                    {
                        Name = "Root",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsRestricted = true,
                        AccessLevel = new AccessLevel
                        {
                            Level = AccessLevelNames.BaseLevel,
                            LevelDescription = "Base level",
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        },
                        Subfolders =facultyFolders

                    };
                    dbContext.Folders.Add(rootFolder);
                };
                dbContext.SaveChanges();

            }


        
        }
    }
}
          

           


        
    
       

    
