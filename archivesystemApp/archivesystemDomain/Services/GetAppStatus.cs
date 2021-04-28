using archivesystemDomain.Entities;

namespace archivesystemDomain.Services
{
    public static class GetAppStatus
    {
        public static string Value(ApplicationStatus status)
        {
            switch (status)
            {
                case ApplicationStatus.Pending:
                    return "Pending";
                case ApplicationStatus.Opened:
                    return "Opened";
                case ApplicationStatus.Closed:
                    return "Closed";
                case ApplicationStatus.InProgress:
                    return "In Progress";
                case ApplicationStatus.Rejected:
                    return "Rejected";
                default:
                    return null;
            }
        }


    }
}