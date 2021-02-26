using System.ComponentModel.DataAnnotations;

namespace archivesystemWebUI.Models
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Faculty")]
        public int FacultyId { get; set; }
    }
}