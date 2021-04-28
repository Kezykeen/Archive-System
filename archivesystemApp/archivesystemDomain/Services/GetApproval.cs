namespace archivesystemDomain.Services
{
    public static class GetApproval
    {
        public static string Value(bool? approval)
        {
            switch (approval)
            {
                case true:
                    return "Approved";
                case false:
                    return "Declined";
                default:
                    return "Not yet";
            }
        }
    }
}