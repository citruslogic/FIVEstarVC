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
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Discharge Date")]
        [IsDateAfter("LastAdmitted", true, ErrorMessage = "Discharge Date must come after or on Last Admitted Date")]
        public DateTime? DischargeDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy", ApplyFormatInEditMode = true)]
        [Display(Name = "Last Admit Date")]
        public DateTime LastAdmitted { get; set; }

        [Display(Name = "Full name")]
        public string FullName { get; set; }

        public string Birthdate { get; set; }

        public string Note { get; set; }

        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch {get; set; }

        [Display(Name = "Reason")]
        public int ProgramTypeID { get; set; }

        public string ErrorMessage { get; set; }
       
    }
}