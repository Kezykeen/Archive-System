using System;

namespace archivesystemWebUI.Models.DataLayers
{
    public class ApprovalDataView
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Remark { get; set; }
        public bool? Approve { get; set; }
        public DateTime? Date { get; set; }
        public DateTime InviteDate { get; set; }
    }
}