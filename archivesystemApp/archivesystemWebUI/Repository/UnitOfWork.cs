using archivesystemDomain.Interfaces;
using System.Threading.Tasks;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
   
        public ITokenRepo TokenRepo { get; }
        public IEmployeeRepository EmployeeRepo { get; }
        public IDeptRepository DeptRepo { get; }
        public IAccessLevelRepository AccessLevelRepo { get; }
        public IFacultyRepository FacultyRepo { get; set; }
        public IAccessDetailsRepository AccessDetailsRepo { get; }
        public IFolderRepo FolderRepo { get; }
        public IFileMetaRepo FileMetaRepo { get; }
        


        public UnitOfWork(
            ApplicationDbContext context,
            IEmployeeRepository employeeRepo,
            IDeptRepository deptRepo,
            IAccessLevelRepository accessLevelRepo,
            IFacultyRepository facultyRepo, 
            ITokenRepo tokenRepo,
            IAccessDetailsRepository accessDetailsRepository,
            IFolderRepo folderRepo,
            IFileMetaRepo fileMetaRepo
           
            )
        {
            _context = context;
            FileMetaRepo = fileMetaRepo;
            TokenRepo = tokenRepo;
            DeptRepo = deptRepo;
            AccessLevelRepo = accessLevelRepo;
            AccessDetailsRepo = accessDetailsRepository;
            EmployeeRepo = employeeRepo;
            FolderRepo = folderRepo;
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