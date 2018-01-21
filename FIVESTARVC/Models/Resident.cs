﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace FIVESTARVC.Models
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

    public class Resident
    {

        public int ResidentID { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }
        [Display(Name = "Service Branch")]
        public ServiceType ServiceBranch { get; set; }
        [Display(Name = "Rank")]
        public string Rank { get; set; }
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
        public virtual ICollection<ProgramEvent> ProgramEvents { get; set; }

        public int? BenefitID { get; set; }
        public virtual ICollection<Benefit> Benefits { get; set; }
    }
}