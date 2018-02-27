using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIVESTARVC.Models
{
    
    public class MilitaryCampaign
    {
        public List<MilitaryCampaign> militaryCampaign = new List<MilitaryCampaign>();

        public int MilitaryCampaignID { get; set; }
        [Display(Name = "Campaign")]
        public string CampaignName { get; set; }

        public virtual ICollection<Resident> Residents { get; set; }

        public IEnumerable<SelectListItem> CampaignIEnum
        {
            get
            {
                return new SelectList(militaryCampaign, "MilitaryCampaignID", "Campaign");

            }

        }
    }
}