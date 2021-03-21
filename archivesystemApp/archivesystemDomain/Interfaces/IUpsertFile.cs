using System.Web;
using archivesystemDomain.Entities;

namespace archivesystemDomain.Interfaces
{
    public interface IUpsertFile
    {
        File Save(File model, HttpPostedFileBase file = null);
    }
}