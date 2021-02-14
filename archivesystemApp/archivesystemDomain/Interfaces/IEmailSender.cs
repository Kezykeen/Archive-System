using System.Threading.Tasks;
using archivesystemDomain.Services;

namespace archivesystemDomain.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(Message message);
    }
}