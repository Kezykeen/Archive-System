using System;
using System.Collections.Generic;
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

        [Required]
        [Display(Name = "Access Level")]
        public int AccessLevelId { get; set; }
        public IEnumerable<AccessLevel> AccessLevel { get; set; }
        public string UploadedById  { get; set; }
        public int FolderId { get; set; }

        public bool Archive { get; set; }

        [Required]
        [Display(Name = "Attach File")]
        [MIMEType(mimeTypes:"docx,doc,pdf,ppt,pptx,zip,xls,xlsx")]
        public HttpPostedFileBase FileBase { get; set; }
      
    }
}