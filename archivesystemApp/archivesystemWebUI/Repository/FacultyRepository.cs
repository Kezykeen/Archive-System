using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Repository
{
    public class FacultyRepository : Repository<Faculty>, IFacultyRepository
    {
        private readonly ApplicationDbContext _context;

        public FacultyRepository(ApplicationDbContext context)
            :base(context)
        {
            _context = context;
        }

        public void Update(Faculty faculty)
        {
            _context.Entry(faculty).State = EntityState.Modified;
        }

        public List<Faculty> GetAllToList()
        {
            return _context.Faculties.ToList();
        }

        public ApplicationDbContext ApplicationDbContext => Context as ApplicationDbContext;
    }
}