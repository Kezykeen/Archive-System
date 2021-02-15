using archivesystemDomain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }


        [Required]
        [Display(Name = "Birthday")]
        public DateTime? DOB { get; set; }


        [Required]
        [Display(Name = "Gender")]
        public Gender? Gender { get; set; }


        [Required]
        [Display(Name = "Marital Status")]
        public MaritalStatus? MaritalStatus { get; set; }


        [Required]
        [Display(Name = "Date Appointed")]
        public DateTime? Appointed { get; set; }
        [Required]
        [Display(Name = "Staff Id")]
        public string StaffId { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }
        public IEnumerable<Department> Departments { get; set; }


        

    }
}