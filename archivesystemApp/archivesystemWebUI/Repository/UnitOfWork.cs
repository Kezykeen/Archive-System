using archivesystemDomain.Interfaces;
using System.Threading.Tasks;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IEmployeeRepository EmployeeRepo { get; }
        public IDeptRepository DeptRepo { get; }
        public IAccessLevelRepository AccessLevelRepo { get; }
        public IFacultyRepository FacultyRepo { get; set; }



        public UnitOfWork(
            ApplicationDbContext context,
            IEmployeeRepository employeeRepo,
            IDeptRepository deptRepo,
            IAccessLevelRepository accessLevelRepo,
            IFacultyRepository facultyRepo
        )
        {
            _context = context;
            DeptRepo = deptRepo;
            AccessLevelRepo = accessLevelRepo;
            EmployeeRepo = employeeRepo;
            FacultyRepo = facultyRepo;
        }


        public int Save()
        {
            return _context.SaveChanges();
        }
        
        public async Task<int> SaveAsync()
        {
           return await _context.SaveChangesAsync();
           
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}