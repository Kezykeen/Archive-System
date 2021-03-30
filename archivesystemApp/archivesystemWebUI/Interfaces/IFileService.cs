using System.Collections.Generic;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface IFileService
    {
        (bool save, FileMetaVm model) Create(FileMetaVm model, HttpPostedFileBase fileBase);
        ICollection<File> GetFiles(int folderId);
        File Details(int id);
        File GetFile(int id, string fileName);
    }
}