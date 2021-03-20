using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace archivesystemWebUI.Services
{
    public class UserAccessService : IUserAccessService
    {
        #region FIELDS
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private const string add = "add";
        private const string update = "update";
        #endregion

        #region PROPERTIES
        public IEnumerable<AccessLevel> AccessLevels { get { return _unitOfWork.AccessLevelRepo.GetAll(); } }
        public IEnumerable<AccessDetail> AccessDetails { get { return _unitOfWork.AccessDetailsRepo.GetAccessDetails(); } }
        #endregion

        #region CONSTRUCTOR
        public UserAccessService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }
        #endregion

        #region MAIN METHODS
        public async Task AddUser(AddUserToAccessViewModel model)
        {
            var user = _unitOfWork.UserRepo.GetUserByMail(model.Email);

            var newAccessDetails = new AccessDetail
            {
                AppUserId = user.Id,
                AccessLevelId = model.AccessLevel,
                AccessCode = await GenerateCode(user, add),
                Status = Status.Active
            };

            _unitOfWork.AccessDetailsRepo.Add(newAccessDetails);
            await _unitOfWork.SaveAsync();
        }

        public AppUser GetUserByEmail(string email)
        {
            return _unitOfWork.UserRepo.GetUserByMail(email);

        }

        public AccessDetail GetById(int? id)
        {
            return _unitOfWork.AccessDetailsRepo.GetAccessDetails().SingleOrDefault(m => m.Id == id.Value);
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
                model.AccessDetails.AccessCode = await GenerateCode(user, update);
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
        #endregion

        #region HELPER METHOD
        private async Task<string> GenerateCode(AppUser user, string method)
        {
            var accessCode = AccessCodeGenerator.NewCode(user.TagId);

            if (method == add)
            {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Access Code",
                    $"Hello {user.Name},\nYour access code is:\n<strong>{accessCode}</strong>.\nThis is confidential. Do not share with anyone.");
            }

            if (method == update)
            {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Access Code (Updated)",
                    $"Hello {user.Name},\nYour new access code is:\n<strong>{accessCode}</strong>.\nThis is confidential. Do not share with anyone.");
            }

            return AccessCodeGenerator.HashCode(accessCode);
        }
        #endregion

    }
}