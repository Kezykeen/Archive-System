using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace archivesystemDomain.Services
{
  public class Message
    {
        public string Destination { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
