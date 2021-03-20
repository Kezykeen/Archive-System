using System;
using System.Collections;
using System.Collections.Generic;
// using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using archivesystemDomain.Entities;
using archivesystemDomain.Interfaces;
using archivesystemDomain.Services;
using archivesystemWebUI.Models.DataLayers;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace archivesystemWebUI.Controllers.Api
{
    public class UsersController : ApiController
    {
     

        private ApplicationUserManager _userManager;
        private readonly IUnitOfWork _unitOfWork;


        public UsersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           
        }



        public UsersController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }



        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Route("api/users")]
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            var employees = _unitOfWork.UserRepo.GetUsersWithDept();
            var response = Mapper.Map<IEnumerable<AppUser>,List<UserDataView>>(employees);

            return Ok(response);

        }


        [HttpGet]
        public IHttpActionResult GetUser(string id)
        {
            return NotFound();
        }


        [HttpDelete]
        public IHttpActionResult DeleteUser(int id)
        {

            var employee = _unitOfWork.UserRepo.Get(id);
            if (employee == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrWhiteSpace(employee.UserId))
            {
                var iduser = UserManager.FindById(employee.UserId);
                UserManager.Delete(iduser);
            }
            _unitOfWork.UserRepo.Remove(employee);
            _unitOfWork.Save();
            return Ok();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) 
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }
           

            base.Dispose(disposing);
        }
    }
}
