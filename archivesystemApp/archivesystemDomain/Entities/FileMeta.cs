using System;

namespace archivesystemDomain.Entities
{
    public class FileMeta
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public File File { get; set; }
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
        public int UploadedById { get; set; }
        public Employee UploadedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}