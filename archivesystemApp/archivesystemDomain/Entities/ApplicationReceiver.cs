using System;

namespace archivesystemDomain.Entities
{
    public class ApplicationReceiver
    {
        public int Id { get; set; }
        public int ReceiverId { get; set; }
        public Department Receiver { get; set; }
        public bool Forwarded { get; set; }
        public bool? Received { get; set; }
        public bool? Approved { get; set; }
        public DateTime TimeSent { get; set; }
        public DateTime? TimeReceived { get; set; }
        public DateTime TimeRejected { get; set; }

    }
}