using System.ComponentModel.DataAnnotations;

namespace archivesystemWebUI.Models
{
    public class AssignDeptsVm
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int? DepartmentId { get; set; }

    }
}