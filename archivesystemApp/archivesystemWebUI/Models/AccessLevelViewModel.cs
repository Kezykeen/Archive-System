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
        [Display(Name = "Name")]
        public string LevelName { get; set; }
    }
}