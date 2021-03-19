using System;
using System.Collections.Generic;

namespace archivesystemDomain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public ICollection<File> Attachments { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public int ApplicationId { get; set; }
        public Application Application { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}