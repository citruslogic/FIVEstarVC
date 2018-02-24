using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIVESTARVC.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace FIVESTARVC.ViewModels
{
    public class ResidentIncomeModel
    {
        //Room

        public List<Room> Rooms = new List<Room>();
        public int SelectedRoomID { get; set; }
        public int RoomID { get; set; }

        public bool IsOccupied {get; set;}
        public int RoomNum { get; set; }


        public IEnumerable<SelectListItem> RoomIEnum
        {
            get
            {
                return new SelectList(Rooms, "RoomID", "RoomNum");

                //return new SelectList(Rooms, "RoomID", "RoomNum", "WingName");
            }

        }

        


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
        public DateTime? Birthdate { get; set; }
        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }

        [Display(Name = "In Veterans Court?")]
        public Boolean InVetCourt { get; set; }
        [Display(Name = "Note")]
        [StringLength(150)]
        public string Note { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }
        public virtual Room Room { get; set; }


        // BENEFIT
        public int BenefitID { get; set; }

        [Display(Name = "Disability Rating")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#}")]
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

    }
}