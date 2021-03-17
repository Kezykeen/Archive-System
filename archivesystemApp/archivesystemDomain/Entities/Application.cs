using System;
using System.Collections.Generic;

namespace archivesystemDomain.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public Ticket ApplicationType { get; set; }
        public string RefNo { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public string Note { get; set; }
        public ApplicationStatus Status { get; set; }
        public bool Archive { get; set; }
        public bool Approve { get; set; }
        public IEnumerable<AppUser> Assignees { get; set; }
        public IEnumerable<ApplicationReceiver> Receivers { get; set; }
        public IEnumerable<Approval> Approvals { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Activity> Activities { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}