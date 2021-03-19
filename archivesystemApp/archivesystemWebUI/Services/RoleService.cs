using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace archivesystemWebUI.Services
{
    public class RoleService : IRoleService
    {
        private static HttpContext  Context => HttpContext.Current;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager RoleManager => Context.GetOwinContext().GetUserManager<ApplicationRoleManager>();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??Context.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IUnitOfWork repo;
        public RoleService(IUnitOfWork repo)
        {
            this.repo = repo; 
        }
        public IEnumerable<ApplicationRole> GetAllRoles()
        {
            return RoleManager.Roles.ToList().Where(x=> x.Name != RoleNames.Admin);
        }

        public void DeleteRole(string roleId)
        {
            var role = RoleManager.FindById(roleId);
            if (role != null)
                RoleManager.Delete(role);
        }

        public async Task<IdentityResult> AddRole (string roleName)
        {
            ApplicationRole role = new ApplicationRole { Name=roleName,CreatedAt=DateTime.Now,UpDatedAt=DateTime.Now};

            IdentityResult result = await RoleManager.CreateAsync(role);
            return result;   
        }
        public IdentityResult AddToRole(string userId, string roleId)
        {
            var role = RoleManager.FindById(roleId);
            var result = UserManager.AddToRole(userId, role.Name);
            return result;
        }
        public IdentityResult AddToRoleByEmail(string userEmail,string roleId)
        {
            var user=repo.UserRepo.GetUserByMail(userEmail);
            if (user == null) return IdentityResult.Failed("user does not exist");
            return AddToRole(user.UserId, roleId);
        }


        public async Task<IdentityResult> EditRole(string oldName, string newName)
        {
           ApplicationRole role=  await RoleManager.FindByNameAsync(oldName);
           role.Name = newName;
           role.UpDatedAt = DateTime.Now;
           var result =await RoleManager.UpdateAsync(role);
           return result;
        }

        public string GetCurrentUserRoles()
        {
            var userId = Context.User.Identity.GetUserId();
            try
            {
                var roles = UserManager.GetRoles(userId);
                return string.Join(",", roles);
            }
            catch (Exception e)
            {
                return e.Message;
            }
            
        }
        public async Task<ICollection<string>> GetUserIdsOfUsersInRole(string roleName)
        {
            var role=await RoleManager.FindByNameAsync(roleName);
            var identityUserRoleObjs =role.Users;
            List<string> userIds = identityUserRoleObjs.Select(x => x.UserId).ToList();
            return userIds;

        }

        public string GetEmployeeName(string userId)
        {
            return repo.UserRepo.Find(x => x.UserId == userId).FirstOrDefault().Name;
        }

        public IEnumerable<RoleMemberData> GetUsersData(ICollection<string> userIds)
        {
            return repo.UserRepo.GetUserDataByUserIds(userIds);
        }

        public IdentityResult RemoveFromRole ( string userId,string roleName)
        {
            var role = RoleManager.FindByName(roleName);
            var result=UserManager.RemoveFromRole(userId, roleName);
            return result;
            //f7fbf102-8f11-43d6-86a1-d34bd0fa7ed2
        }

    }
}