using System.Threading.Tasks;
using archivesystemDomain.Services;

namespace archivesystemDomain.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string destination, string subject, string body);
    }
}