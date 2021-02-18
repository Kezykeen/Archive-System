using System.Collections.Generic;
using archivesystemDomain.Entities;

namespace archivesystemDomain.Interfaces
{
    public interface IDeptRepository : IRepository<Department>
    {
        IEnumerable<Department> GetDeptWithFaculty();
        List<Department> GetAllToList();
        void Update(Department department);
    }
}