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

            if (!dbContext.Folders.Any())
            {
                
                //Seed Root folder
                dbContext.Folders.Add(new Folder
                { 
                    Name = "Root",
                    CreatedAt=DateTime.Now,
                    UpdatedAt=DateTime.Now ,
                    IsDeletable=false,
                    AccessLevel= new AccessLevel
                    {
                        Level = AccessLevelNames.BaseLevel,
                        LevelDescription = "Base level",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                });
                dbContext.SaveChanges();

                //Add root folder path
                var rootFolder = dbContext.Folders.SingleOrDefault(x => x.Name == "Root");
                rootFolder.Path = $"{rootFolder.Id}#{rootFolder.Name}";
                dbContext.SaveChanges();
               
            }


        }
       

    }
}