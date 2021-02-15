using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace archivesystemWebUI.Models
{
    public class CreateAccessLevelViewModel
    {
        [Required(ErrorMessage ="Please input the level as a digit(1,2,3 ....")]
        [Display(Name = "Level")]
        public string Level { get; set; }

        [Required(ErrorMessage = "Please input the name of the Access Level")]
        [Display(Name = "Description")]
        public string LevelDescription { get; set; }

    }

    public class AddUserToAccessViewModel
    {
        [Required(ErrorMessage ="Enter a valid identification for the user.")]
        [Display(Name = "Email Address")]
        public string UserIdentification { get; set; }

        [Required(ErrorMessage = "Choose an access level.")]
        [Display(Name = "Access Level")]
        public string AccessLevel { get; set; }

        public IEnumerable<AccessLevel> AccessLevels { get; set; }
    }
}