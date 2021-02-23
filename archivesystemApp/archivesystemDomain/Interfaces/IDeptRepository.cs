using System.Collections.Generic;
using archivesystemDomain.Entities;

namespace archivesystemDomain.Interfaces
{
    public interface IDeptRepository : IRepository<Department>
    {
        List<Department> GetAllToList();
        void Update(Department department);
    }
}