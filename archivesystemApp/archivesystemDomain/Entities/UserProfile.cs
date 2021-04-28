namespace archivesystemDomain.Entities
{
    public class UserProfile
    {
        public int Id { get; set; }
        public int SignatureId { get; set; }
        public File Signature { get; set; }
    }
}