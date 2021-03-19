using archivesystemDomain.Entities;
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

    public class RoleMemberData
    {
        public string UserId { get; set; }
        public Department Department { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class GlobalConstants
    {
        public const string IsAccessValidated = "IsAccessValidated";
        public const string LastVisit = "LastVisit";
        public const string userHasNoAccesscode = " userHasNoAccesscode";
        public const string RootFolderName = "Root";
        public const int MaxFolderDepth = 7;

    }
}