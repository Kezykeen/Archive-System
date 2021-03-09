using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Models
{
    public class FacultyViewModel
    {
        public int Id { get; set; }

        [Remote("FacultyNameCheck", "Faculty", AdditionalFields = "Id", HttpMethod = "POST", ErrorMessage = "Name already exist")]
        [Required(ErrorMessage = "Enter name")]
        public string Name { get; set; }
    }
}