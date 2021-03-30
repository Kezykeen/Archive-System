using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;
using archivesystemWebUI.Infrastructures;

namespace archivesystemWebUI.Models
{
    public class ApplicationVm
    {

        [Required]
        [Display(Name = "Type")]
        public int ApplicationTypeId { get; set; }
        public IEnumerable<Ticket> ApplicationTypes { get; set; }

        [Required]
        [Display(Name = "Send To")]
        public int DepartmentId { get; set; }
        public IEnumerable<Department> Departments { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(10)]
        public string Note { get; set; }

        [Display(Name = "Attach File")]
        [MaxFileSizeMb(50)]
        [MIMEType(mimeTypes: "docx,doc,pdf,ppt,pptx,zip,xls,xlsx")]
        public HttpPostedFileBase FileBase { get; set; }

        public File Attachment { get; set; }
        public ICollection<AppUser> Assignees { get; set; }
        public ICollection<ApplicationReceiver> Receivers { get; set; }
        public ICollection<Approval> Approvals { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}