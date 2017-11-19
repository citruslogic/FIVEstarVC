using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public enum CampaignType
    {
        [Display(Name = "Gulf War")]
        GULF_WAR,
        [Display(Name = "Vietnam war")]
        VIETNAM,
        [Display(Name = "Desert Storm")]
        DESERT_STORM,
        [Display(Name = "Afghanistan War")]
        AFGHANISTAN

    }
    public class MilitaryCampaign
    {
        public int ID { get; set; }
        [Display(Name = "Campaign")]
        public CampaignType? CampaignName { get; set; }

        public virtual ResidentCampaign ResidentCampaign { get; set; }
    }
}