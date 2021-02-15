using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Repository
{
    public class DeptRepository : Repository<Department>, IDeptRepository
    {
        private readonly ApplicationDbContext _context;

        public DeptRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public IEnumerable<Department> GetDeptWithFaculty()
        {
            return _context.Departments.Include(f => f.Faculty).ToList();
        }

        public void Update(Department department)
        { 
            _context.Entry(department).State = EntityState.Modified;
        }

        public List<Department> GetAllToList()
        {
            return _context.Departments.Include(x => x.Faculty).ToList();
        }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}