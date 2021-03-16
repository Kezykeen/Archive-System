using archivesystemDomain.Entities;
using archivesystemWebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace archivesystemWebUI.Interfaces
{
    public interface IUserAccessService
    {
        IEnumerable<AccessDetail> AccessDetails { get; }
        IEnumerable<AccessLevel> AccessLevels { get; }

        Task AddUser(AddUserToAccessViewModel model);
        AddUserToAccessViewModel AddUserModel();
        Task Delete(int id);
        void EditUserModel(int id, out EditUserViewModel model, out AccessDetail accessDetails);
        AccessDetail GetByNullableId(int? id);
        Task Update(EditUserViewModel model);
        void ValidateEmail(string Email, out AccessDetail accessDetails, out Employee employee);
    }
}
