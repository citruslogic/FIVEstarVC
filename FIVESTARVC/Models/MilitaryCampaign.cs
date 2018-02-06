using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    
    public class MilitaryCampaign
    {
        public int MilitaryCampaignID { get; set; }
        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        public virtual ICollection<Resident> Residents { get; set; }
    }
}