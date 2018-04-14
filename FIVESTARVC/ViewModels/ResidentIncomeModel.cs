using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIVESTARVC.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using FIVESTARVC.Validators;

namespace FIVESTARVC.ViewModels
{
    public class ResidentIncomeModel
    {
        //Room
        [Required]
        public int RoomNumber { get; set; }

        // RESIDENT
        public int ResidentID { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        [Birthdate(ErrorMessage = "Birthdate must not be in the future.")]
        [Age(ErrorMessage = "Applicant must be 18 years or older.")]
        public DateTime Birthdate { get; set; }

        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }

        /* March changes to the model - 3/15/2018 */
        [Display(Name = "Gender")]
        public GenderType Gender { get; set; }
        [Display(Name = "Ethnicity")]
        public EthnicityType Ethnicity { get; set; }
        [Display(Name = "Religion")]
        public ReligionType Religion { get; set; }

        [Display(Name = "Home of Record")]
        [ForeignKey("StateTerritory")]
        public int StateTerritoryID { get; set; }
        public virtual StateTerritory StateTerritory { get; set; }

        [Display(Name = "In Veterans Court?")]
        public Boolean InVetCourt { get; set; }
        [Display(Name = "Non-combat?")]
        public Boolean IsNoncombat { get; set; }
        [Display(Name = "Note")]
        [StringLength(150)]
        public string Note { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }
        public virtual Room Room { get; set; }


        // BENEFIT
        public int BenefitID { get; set; }

        [Display(Name = "Disability Rating")]
        [RegularExpression(@"(^100(\.0{1,2})?%$)|(^([1-9]([0-9])?|0)(\.[0-9]{1,2})?%$)", ErrorMessage = "Percentage only, minimum 0 and maximum 100.")]
        public string DisabilityPercentage { get; set; }

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
        public String OtherDescription { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Display(Name = "Other (Amount)")]
        public decimal? Other { get; set; }

    }
}