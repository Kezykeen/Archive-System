﻿using archivesystemDomain.Entities;
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
        Task<(string, Exception)> AddUser(AddUserToAccessViewModel model);
        AddUserToAccessViewModel AddUserModel();
        Task<string> Delete(int id);
        void EditUserModel(int id, out EditUserViewModel model, out AccessDetail accessDetails);
        AccessDetail GetByNullableId(int? id);
        AppUser GetUserByEmail(string email);
        AccessDetail GetByAppUserId(int id);
        Task<(string, Exception)> UpdateUser(EditUserViewModel model);
    }
}
