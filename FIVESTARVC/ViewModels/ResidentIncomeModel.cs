﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using FIVESTARVC.DAL;
using FIVESTARVC.Models;

namespace FIVESTARVC.ViewModels
{
    public enum ServiceType
    {
        [Description("Air Force")]
        [Display(Name = "Air Force")]
        AIRFORCE,
        [Description("Army")]
        [Display(Name = "Army")]
        ARMY,
        [Description("Coast Guard")]
        [Display(Name = "Coast Guard")]
        COASTGUARD,
        [Description("Marines")]
        [Display(Name = "Marines")]
        MARINES,
        [Description("Navy")]
        [Display(Name = "Navy")]
        NAVY
    }

    public class ResidentIncomeModel
    {



        /* RESIDENT */
        public int ResidentID { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Birthdate { get; set; }
        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }
        [Display(Name = "Resident has PTSD?")]
        public Boolean HasPTSD { get; set; }
        [Display(Name = "In Veterans Court")]
        public Boolean InVetCourt { get; set; }
        [Display(Name = "Room Number")]
        [ForeignKey("Room")]
        public int? RoomID { get; set; }
        [Display(Name = "Note")]
        [StringLength(150)]
        public string Note { get; set; }

        /* BENEFITS */
        public int BenefitID { get; set; }

        [Display(Name = "Disability Rating (%)")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
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