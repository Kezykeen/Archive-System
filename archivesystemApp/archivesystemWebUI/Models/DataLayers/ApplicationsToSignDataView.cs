using System.Collections.Generic;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Models.DataLayers
{
    public class ApplicationsToSignDataView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RefNo { get; set; }
        public string Type { get; set; }
        public ApprovalDataView Approval { get; set; }
       
        
    }
}