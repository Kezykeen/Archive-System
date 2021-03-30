using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using archivesystemDomain.Entities;
using archivesystemDomain.Services;
using archivesystemWebUI.Infrastructures;
using archivesystemWebUI.Models;

namespace archivesystemWebUI.Interfaces
{
    public interface IUserService
    {
        Task<(bool save, string msg)> Create(AppUser user, UserUniqueProps uniqueProps);
        (bool save, string msg) Update(UpdateUserVm model );
        bool UpdateUserId(string email, string id);
        (bool found, AppUser result) Get(int id);
        (bool found, AppUser result) GetById(string id);
        (bool found, AppUser result) GetByMail(string email);
        IEnumerable<AppUser> GetAll();
        Token GetToken(string code);
        Task<(bool sent, string msg)> ResendConfirmation(int id, string email, string name);
        bool Remove(int id);
        void CompleteReg(AppUser user);
        void RemoveToken(Token token);
        Task<(bool found, IEnumerable result)> GetDeptOfficers(int id, string searchTerm);
        bool NameExists(string name, int? userId);
        bool TagIdExists(string tagId, int? userId);
        bool PhoneExists(string phone, int? userId);
        bool EmailExists(string email, int? userId);
    }
}