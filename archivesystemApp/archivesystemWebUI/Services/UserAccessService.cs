using archivesystemDomain.Entities;
using archivesystemWebUI.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using archivesystemDomain.Interfaces;

namespace archivesystemWebUI.Services
{
    public class UserAccessService : IUserAccessService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccessLevelService _accessLevelService;

        public UserAccessService(IUnitOfWork unitOfWork, IAccessLevelService accessLevelService)
        {
            _unitOfWork = unitOfWork;
            _accessLevelService = accessLevelService;
        }

        public IEnumerable<AccessLevel> AccessLevels { get { return _unitOfWork.AccessLevelRepo.GetAll(); } }
        public IEnumerable<AccessDetail> AccessDetails { get { return _unitOfWork.AccessDetailsRepo.GetAccessDetails(); } }
        
        public async Task AddUser(AddUserToAccessViewModel model)
        {
            var employee = _unitOfWork.EmployeeRepo.GetEmployeeByMail(model.Email);

            var newAccessDetails = new AccessDetail
            {
                EmployeeId = employee.Id,
                AccessLevelId = model.AccessLevel,
                AccessCode = AccessCodeGenerator.NewCode(employee.StaffId),
                Status = Status.Active
            };

            _unitOfWork.AccessDetailsRepo.Add(newAccessDetails);
            await _unitOfWork.SaveAsync();
        }


        public void GetById(int id, out AccessDetail accessDetails)
        {
            accessDetails = _unitOfWork.AccessDetailsRepo.Get(id);

        }
        
        public AccessDetail GetByNullableId(int? id)
        {
            return   _unitOfWork.AccessDetailsRepo.GetAccessDetails().SingleOrDefault(m => m.Id == id.Value);


        }


        public AddUserToAccessViewModel AddUserModel()
        {
            return new AddUserToAccessViewModel { AccessLevels = AccessLevels };

        }
        
        public void EditUserModel(int id, out EditUserViewModel model, out AccessDetail accessDetails)
        {
            accessDetails = _unitOfWork.AccessDetailsRepo.Get(id);

            model = new EditUserViewModel
            {
                RegenerateCode = CodeStatus.No,
                AccessDetails = accessDetails,
                AccessLevels = AccessLevels
            };

        }

        public async Task Update(EditUserViewModel model)
        {
            if (model.RegenerateCode == CodeStatus.Yes)
            {
                var employee = _unitOfWork.EmployeeRepo.Get(model.AccessDetails.EmployeeId);
                model.AccessDetails.AccessCode = AccessCodeGenerator.NewCode(employee.StaffId);
            }

            _unitOfWork.AccessDetailsRepo.EditDetails(model.AccessDetails);
            await _unitOfWork.SaveAsync();
        }
        
        public async Task Delete(int id)
        {
            var accessDetails = _unitOfWork.AccessDetailsRepo.Get(id);
            _unitOfWork.AccessDetailsRepo.Remove(accessDetails);
            await _unitOfWork.SaveAsync();
        }

        public void ValidateEmail(string Email, out AccessDetail accessDetails, out Employee employee)
        {
           var getEmployee = _unitOfWork.EmployeeRepo.GetEmployeeByMail(Email);
           employee = getEmployee;
           accessDetails = _unitOfWork.AccessDetailsRepo.GetByEmployeeId(getEmployee.Id);
        }
    }
}