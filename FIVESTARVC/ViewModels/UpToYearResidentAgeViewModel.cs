using System;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.ViewModels
{
    public class UpToYearResidentAgeViewModel
    {
        public int ResidentID { get; set; }

        [Display(Name="Full Name")]
        public string FullName { get; set; }

        public int? Age { get; set; }

        public string Birthdate { get; set; }

        [Display(Name="Date Admitted")]
        public string DateAdmitted { get; set; }

        [Display(Name="Date Discharged")]
        public string DateDischarged { get; set; }
    }
}