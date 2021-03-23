using System;

namespace archivesystemDomain.Entities
{
    public class Approval
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; }
        public string Remark { get; set; }
        public bool? Approve { get; set; }
        public DateTime? Date { get; set; }
        public DateTime InviteDate { get; set; }
    }
}