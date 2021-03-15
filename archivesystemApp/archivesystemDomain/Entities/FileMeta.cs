using System;

namespace archivesystemDomain.Entities
{
    public class FileMeta
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UploadedById { get; set; }
        public ApplicationUser UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}