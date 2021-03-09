using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemDomain.Services
{
    public class AccessLevelNames
    {
        public const string BaseLevel = "1";
    }

    public enum AllowableFolderDepth
    {
        Max=7
    }

    public class FolderPath
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}