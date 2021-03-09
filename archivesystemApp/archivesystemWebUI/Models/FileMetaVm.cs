using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemWebUI.Infrastructures;

namespace archivesystemWebUI.Models
{
    public class FileMetaVm
    {

        [Required]
        [MinLength(2)]
        public string Title { get; set; }
        public File File { get; set; }
        public int UploadedById  { get; set; }
        public int FolderId { get; set; }
        [Required]
        [Display(Name = "Attach File")]
        [MaxFileSizeMb(250, ErrorMessage = "File Exceeds the Max Size (250Mb)")]
        public HttpPostedFileBase FileBase { get; set; }
      
    }
}