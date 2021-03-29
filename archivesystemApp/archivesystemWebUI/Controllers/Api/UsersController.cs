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
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models.DataLayers;
using AutoMapper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace archivesystemWebUI.Controllers.Api
{
    public class UsersController : ApiController
    {
        private readonly IUserService _userService;


        private ApplicationUserManager _userManager;


        public UsersController(IUserService userService)
        {
            _userService = userService;
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
            var employees = _userService.GetAll();
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

            var delete = _userService.Remove(id);
            if (delete) return Ok();
            
            return NotFound();
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
