using System;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Infrastructures
{
    public class UpsertFile
    {
        public static void Save(FileMetaVm model, HttpPostedFileBase file = null)
        {
            if (model.File != null)
            {
                model.File.Name = $"{Guid.NewGuid():N}.{System.IO.Path.GetFileName(file.FileName)?.Split('.').Last()}";

                model.File.ContentType = file.ContentType;
                using (var reader = new System.IO.BinaryReader(file.InputStream))
                {
                    model.File.Content = reader.ReadBytes(file.ContentLength);
                }
            }
            else
            {
                model.File = new File()
                {
                    Name = $"{Guid.NewGuid():N}.{System.IO.Path.GetFileName(file.FileName)?.Split('.').Last()}",
                    ContentType = file.ContentType
                };
                using (var reader = new System.IO.BinaryReader(file.InputStream))
                {
                    model.File.Content = reader.ReadBytes(file.ContentLength);
                }

            }
        }
    }
}