using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Interfaces;
using archivesystemWebUI.Interfaces;
using archivesystemWebUI.Models;
using Microsoft.AspNet.Identity;

namespace archivesystemWebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }
        public ActionResult Index()
        {
            var model = _homeService.GetAllClasses();
            return View(model);
        }

        public ActionResult EmployeeDashboard()
        {          


            return View();
        }

      
    }
}