using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace archivesystemDomain.Entities
{
    public class Application
    {
        public int Id { get; set; }

        public int ApplicationTypeId { get; set; }
        public Ticket ApplicationType { get; set; }

        [Index("TitleIndex")]
        [StringLength(50)]
        public string Title { get; set; }

        [Index("RefNoIndex")]
        [StringLength(50)]
        public string RefNo { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public string Note { get; set; }
        public ApplicationStatus Status { get; set; }
        public bool Archive { get; set; }
        public bool? Approve { get; set; }
        public int? AttachmentId { get; set; }
        public File Attachment { get; set; }
        public ICollection<Signer> Signers { get; set; }
        public ICollection<ApplicationReceiver> Receivers { get; set; }
        public ICollection<Approval> Approvals { get; set; }
        public ICollection<Activity> Activities { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}