namespace archivesystemDomain.Interfaces
{
    public interface ITokenGenerator
    {
        string Code(int userId);
    }
}