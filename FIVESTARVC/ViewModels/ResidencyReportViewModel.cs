using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.ViewModels
{
    public class ResidencyReportViewModel
    {
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Admitted")]
        public List<string> AdmitDates { get; set; }

        [Display(Name = "Discharge")]
        public string DischargeDate { get; set; }

        public bool IsCurrent { get; set; }

        [Display(Name = "Days In Residence")]
        public int DaysInResidence { get; set; }

        [Display(Name = "Total Length")]
        public int TotalLength { get; set; }

        public double MonthsStayed { get; set; }
    }
}