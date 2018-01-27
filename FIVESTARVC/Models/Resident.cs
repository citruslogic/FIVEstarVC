using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


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

        public int ResidentID { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Birthdate { get; set; }
        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }
        [Display(Name = "Rank")]
        public string Rank { get; set; }
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }
        [Display(Name = "In Veterans Court")]
        public Boolean InVetCourt { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }

        public int? BenefitID { get; set; }
        public virtual ICollection<Benefit> Benefits { get; set; }
    }
}
