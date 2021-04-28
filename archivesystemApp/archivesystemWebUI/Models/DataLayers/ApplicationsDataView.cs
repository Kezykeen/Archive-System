using System;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models.DataLayers
{
    public class ApplicationsDataView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string RefNo { get; set; }
        public string Type { get; set; }
        public string Receiver { get; set; }
        public string Status { get; set; }
        public string Approval { get; set; }
        public string SubmissionDate { get; set; }
    }
}
