using FIVESTARVC.Models;
using FIVESTARVC.Validators;
using System;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.ViewModels
{
    public class DischargeViewModel
    {
        public int ResidentID { get; set; }

        [Required]
        [DischargeDateCheck]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Discharge Date")]
        public DateTime? DischargeDate { get; set; }

        [Display(Name = "Full name")]
        public string FullName { get; set; }

        public string Birthdate { get; set; }

        public string Note { get; set; }

        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch {get; set; }

        [Display(Name = "Reason")]
        public int ProgramTypeID { get; set; }

    }
}