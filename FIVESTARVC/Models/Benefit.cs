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
        [RegularExpression(@"(^100(\.0{1,2})?%$)|(^([1-9]([0-9])?|0)(\.[0-9]{1,2})?%$)", ErrorMessage = "Percentage only, minimum 0 and maximum 100.")]
        public string DisabilityPercentage { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Disability Amount")]
        public decimal? DisabilityAmount { get; set; }

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

        
        [Display(Name = "Food stamps")]
        public bool FoodStamp { get; set; }
        
        /* Other forms of disability as income */
        [Display(Name = "Other Income (Description)")]
        public string OtherDescription { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Other (Amount)")]
        public decimal? Other { get; set; }
        /***************************************/

    }
}