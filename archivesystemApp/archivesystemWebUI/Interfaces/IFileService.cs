using System.Collections.Generic;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;
using archivesystemWebUI.Models.FolderModels;

namespace archivesystemWebUI.Interfaces
{
    public interface IFileService
    {
        (bool save, FileMetaVm model) Create(FileMetaVm model, HttpPostedFileBase fileBase);
        ICollection<File> GetFiles(int folderId);
        File Details(int id);
        File GetFile(int id, string fileName);
        RequestResponse<string> DeleteFile(int id);
        RequestResponse<string> ArchiveFile(int fileId);
    }
}