using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;

namespace archivesystemDomain.Services
{
    class FacultyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FacultyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<Faculty> GetFacultyData()
        {
            var faculty = _unitOfWork.FacultyRepo.GetAllToList();
            
            return faculty;
        }
    }
}
