using archivesystemDomain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace archivesystemWebUI.Models
{
    public class AddUserToAccessViewModel
    {
        [Required(ErrorMessage ="Enter a valid identification for the user.")]
        [Display(Name = "Email Address")]
        [Remote(nameof(Controllers.UserAccessController.ValidateEmail), "UserAccess", HttpMethod = "POST")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Choose an access level.")]
        [Display(Name = "Access Level")]
        public int AccessLevel { get; set; }

        public IEnumerable<AccessLevel> AccessLevels { get; set; }
    }
}