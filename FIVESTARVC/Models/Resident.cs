using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using FIVESTARVC.DAL;

namespace FIVESTARVC.Models
{
    /* May be removed in the future in favor of a separate entity. 
     * The entity name could be ServiceType with ServiceTypeID as 
     * a property to this entity, Resident. 
     * - Frank Butler (1/27/2018) */
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

    public class Resident
    {
        private ResidentContext db = new ResidentContext();

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

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }

        [ForeignKey("Benefits")]
        public int? BenefitID { get; set; }

        public virtual ICollection<Benefit> Benefits { get; set; }

        public Boolean isCurrent(Resident resident)
        {
            var current = db.ProgramEvents;

            int ID = resident.ResidentID;

            Boolean internalBool = false;

            foreach (var ProgramEvent in current)
            {
                if (ID == ProgramEvent.ResidentID)
                {
                    if (ProgramEvent.ProgramTypeID == 7 //admission
                    || ProgramEvent.ProgramTypeID == 9 //re-admit
                    || ProgramEvent.ProgramTypeID == 5)
                    {
                        internalBool = true;
                    }

                    if (ProgramEvent.ProgramTypeID == 2 //graduation
                    || ProgramEvent.ProgramTypeID == 13 //discharge
                    || ProgramEvent.ProgramTypeID == 14 //discharge
                    || ProgramEvent.ProgramTypeID == 15)
                    {
                        internalBool = false;
                    }
                }
            }
            return internalBool;
        }
        public virtual Room Room { get; set; }
    }
}
