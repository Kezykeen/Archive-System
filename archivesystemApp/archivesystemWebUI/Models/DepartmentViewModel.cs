using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Models
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Enter name")]
        [Remote("DepartmentNameCheck", "Department", AdditionalFields = "Id", HttpMethod = "POST", ErrorMessage = "Name already exist")]
        public string Name { get; set; }

        [Display(Name = "Faculty")]
        [Required(ErrorMessage = "Choose a faculty")]
        public int FacultyId { get; set; }

        public IEnumerable<Faculty> Faculties { get; set; }
    }
}