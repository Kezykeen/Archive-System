using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemDomain.Services
{
    public static class AccessCodeGenerator
    {
        public static string NewCode(string staffId)
        {
            var guid = Guid.NewGuid().ToString("N").Substring(0, 6);
            var accessCode = guid + staffId;
            return accessCode;
        }
    }
}
