using System;

namespace archivesystemDomain.Entities
{
    public class Signer
    { 
        public int Id { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime InviteTime { get; set; }
    }
}