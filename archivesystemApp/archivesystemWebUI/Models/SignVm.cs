using System.ComponentModel.DataAnnotations;

namespace archivesystemWebUI.Models
{
    public class SignVm
    {
        [Required]
        public int AppId { get; set; }

        [Required]
        [Display(Name = "Remark")]
        public string Remark { get; set; }
    }
}