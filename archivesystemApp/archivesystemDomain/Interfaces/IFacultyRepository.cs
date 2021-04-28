using System.Collections.Generic;
using archivesystemDomain.Entities;

namespace archivesystemDomain.Interfaces
{
    public interface IFacultyRepository : IRepository<Faculty>
    {
        List<Faculty> GetAllToList();
        void Update(Faculty faculty);
    }
}