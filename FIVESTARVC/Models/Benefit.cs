using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public class Benefit
    {

        public int BenefitID { get; set; }

        [Display(Name = "Disability Rating (%)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        public double DisabilityPercentage { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Total Benefit Amount")]
        public decimal TotalBenefitAmount { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal SSI { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal SSDI { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Food stamps")]
        public decimal FoodStamp { get; set; }
        
        /* Other forms of disability as income */
        [Display(Name = "Other Income")]
        public String OtherDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Other (Amount)")]
        public decimal? Other { get; set; }
        /***************************************/

        public virtual Resident Resident { get; set; }

    }
}