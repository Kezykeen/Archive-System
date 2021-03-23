using System.ComponentModel.DataAnnotations;

namespace archivesystemWebUI.Models
{
    public class AssignUsersVm
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Assign Users")]
        public string[] UserIds { get; set; }
        
    }
}