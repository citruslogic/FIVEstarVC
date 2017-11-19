using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace FIVESTARVC.Models
{
    public enum ServiceType
    {
        [Display(Name = "Army")]
        AIRFORCE,
        [Display(Name = "Marines")]
        ARMY,
        [Display(Name = "Navy")]
        COASTGUARD,
        [Display(Name = "Coast Guard")]
        MARINES,
        [Display(Name = "Air Force")]
        NAVY
    }
    public class Resident
    {

        public int ID { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
