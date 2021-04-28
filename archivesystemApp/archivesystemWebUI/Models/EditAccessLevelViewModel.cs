using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class EditAccessLevelViewModel
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string LevelDescription { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}