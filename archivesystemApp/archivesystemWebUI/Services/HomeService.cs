using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleService _roleService;
        private readonly IDeptRepository _deptRepository;
        private readonly IFacultyRepository _facultyRepository;
        private readonly IAccessDetailsRepository _accessDetailsRepository;
        public HomeService(IUserRepository userRepository, IRoleService roleService, IDeptRepository deptRepository, IFacultyRepository facultyRepository, IAccessDetailsRepository accessDetailsRepository)
        {
            _userRepository = userRepository;
            _roleService = roleService;
            _deptRepository = deptRepository;
            _facultyRepository = facultyRepository;
            _accessDetailsRepository = accessDetailsRepository;
        }

        public CardsViewModel GetAllClasses()
        {
            var model = new CardsViewModel
            {
                Appusers = _userRepository.GetAll().OrderBy(m => m.Name),
                Roles = _roleService.GetAllRoles(),
                Departments = _deptRepository.GetAll(),
                Faculties = _facultyRepository.GetAll(),
                AccessDetails = _accessDetailsRepository.GetAccessDetails().OrderByDescending(m => m.AccessLevel.Level)

            };
            return model;
        }
    }
}