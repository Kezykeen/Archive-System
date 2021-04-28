using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace archivesystemWebUI.Models
{
    public class EnrollViewModel
    {

        [Required]
        [MinLength(2)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [MinLength(2)]
        [Display(Name = "Last Name")]
        [Remote("IsNameTaken", "Users", AdditionalFields = "LastName", ErrorMessage = "Name Already taken!")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        [Remote("IsEmailTaken", "Users", ErrorMessage = "Email Already taken!")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        [Remote("IsPhoneTaken", "Users", ErrorMessage = "Phone Number Already taken!")]
        public string Phone { get; set; }


        [Required]
        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }

        [Required]
        [Display(Name = "Designation")]
        public Designation? Designation { get; set; }
       
        [Required]
        [Display(Name = "ID")]
        [Remote("IsIdTaken", "Users", ErrorMessage = "Id Already taken!")]
        public string TagId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public IEnumerable<Department> Departments { get; set; }
    
    }
}