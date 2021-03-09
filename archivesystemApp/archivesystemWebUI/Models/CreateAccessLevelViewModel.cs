using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace archivesystemWebUI.Models
{
    public class CreateAccessLevelViewModel
    {
        [Required(ErrorMessage ="Please input the level as a digit(1,2,3 ....")]
        [Display(Name = "Level")]
        [Remote(nameof(archivesystemWebUI.Controllers.AccessLevelController.LevelAvailable), "AccessLevel", HttpMethod = "POST", ErrorMessage = "Access Level already exists. Please enter a different Level!")]
        public string Level { get; set; }

        [Required(ErrorMessage = "Please input the description of the Access Level")]
        [Display(Name = "Description")]
        public string LevelDescription { get; set; }
    }
}