using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIVESTARVC.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIVESTARVC.ViewModels
{
    public class DischargeResidentReason
    {
        public int ResidentID { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Birthdate { get; set; }
        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }


        public int ProgramTypeID { get; set; }

    }
}