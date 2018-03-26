using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public class Benefit
    {

        public int BenefitID { get; set; }

        [Display(Name = "Disability Rating")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#}")]
        [Range(0,100, ErrorMessage = "Disability rating must be a minimum of 0 and cannot exceed 100.")]
        public double? DisabilityPercentage { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Total Benefit Amount")]
        public decimal? TotalBenefitAmount { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal? SSI { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal? SSDI { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Food stamps")]
        public decimal? FoodStamp { get; set; }
        
        /* Other forms of disability as income */
        [Display(Name = "Other Income (Description)")]
        public String OtherDescription { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Other (Amount)")]
        public decimal? Other { get; set; }
        /***************************************/

    }
}