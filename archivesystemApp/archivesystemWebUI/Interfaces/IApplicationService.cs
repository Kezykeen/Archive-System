using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface IApplicationService
    {
        IEnumerable<Ticket> GetApplicationTypes(Designation designation);
        bool Create(ApplicationVm model, HttpPostedFileBase fileBase, AppUser user);
        Task<(bool reject, string msg)> Reject(SignVm model);
        Task<(bool accept, string msg)> Accept(int id);
        Task<(bool sign, string msg)> Sign(SignVm model);
        Task<(bool decline, string msg)> Decline(SignVm model);
        Task<(bool sent, string msg )> SendToHod(Application application, AppUser user);
        Task<(bool approve, string msg)> SignApprove(SignVm model);
        Task<(bool forward, string msg)> SignForward(SignVm model);
        Task<(bool disapprove, string msg)> Disapprove(SignVm model);
        ( bool found, Application result )GetApplication(int id);
        (bool secetary, bool hod, bool deptOfficer, bool appOwner) DoCheck(Application application, AppUser user);
        Task<(bool assign, string msg, Application application)> AssignUsers(AssignUsersVm model);
        Task<(bool assign, string msg, Application application)> AssignToDept(AssignDeptsVm model);
        (bool archive, string msg) Archive(int id);
        File GetFile(int id, string fileName);
        IEnumerable<Application> UserApplications();
        IEnumerable<Application> IncomingAppsApplications(bool forwarded = false, bool? received = null, bool archived = false);
        IEnumerable<Application>ApplicationsToSign(bool? signed = null);
        IEnumerable<Application> ApplicationsToApprove(bool? signed = null, bool? approved = null, bool sendToHead = true);
    }
}