using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using archivesystemDomain.Entities;

namespace archivesystemWebUI.Models
{
    public class TicketViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Ticket Code")]
        [StringLength(3)]
        public string Acronym { get; set; }
       

        [Required]
        public Designation? Designation { get; set; }
        [Required]
        public WorkFlow? WorkFlow { get; set; }
        [Required]
        public Status? Status { get; set; }
    }
}