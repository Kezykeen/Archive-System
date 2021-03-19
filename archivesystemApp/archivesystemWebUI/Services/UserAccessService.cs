using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace archivesystemWebUI.Services
{
    public class UserAccessService : IUserAccessService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserAccessService(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<AccessLevel> AccessLevels { get { return _unitOfWork.AccessLevelRepo.GetAll(); } }
        public IEnumerable<AccessDetail> AccessDetails { get { return _unitOfWork.AccessDetailsRepo.GetAccessDetails(); } }

        public async Task AddUser(AddUserToAccessViewModel model)
        {
            var user = _unitOfWork.UserRepo.GetUserByMail(model.Email);

            var accessCode = AccessCodeGenerator.NewCode(user.TagId);

            var newAccessDetails = new AccessDetail
            {
                AppUserId = user.Id,
                AccessLevelId = model.AccessLevel,
                AccessCode = AccessCodeGenerator.HashCode(accessCode),
                Status = Status.Active
            };

            _unitOfWork.AccessDetailsRepo.Add(newAccessDetails);
            await _unitOfWork.SaveAsync();
        }


        public AppUser GetUserByEmail(string email)
        {
            return  _unitOfWork.UserRepo.GetUserByMail(email);

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
                var user = _unitOfWork.UserRepo.Get(model.AccessDetails.AppUserId);
                model.AccessDetails.AccessCode = AccessCodeGenerator.NewCode(user.TagId);
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



    }
}