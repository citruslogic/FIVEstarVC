using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FIVESTARVC.ViewModels
{
    public class UpToYearResidentAgeViewModel
    {
        public int Total { get; set; }

        public List<IGrouping<int, SelectedResident>> Residents { get; set; } = new List<IGrouping<int, SelectedResident>>();
    }

    public class SelectedResident
    {
        public int ResidentID { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        public int? Age { get; set; }

        public string Birthdate { get; set; }

        [Display(Name = "Date Admitted")]
        public DateTime DateAdmitted { get; set; }

        public string ShortDateAdmitted
        {
            get
            {
                return DateAdmitted.ToShortDateString();
            }
        }

        [Display(Name = "Date Discharged")]
        public string DateDischarged { get; set; }

    }
}