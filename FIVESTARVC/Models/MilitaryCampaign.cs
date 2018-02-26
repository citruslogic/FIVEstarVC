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
        [Required]
        [RegularExpression("^\\s?([a-zA-Z]+\\s?){1,3}$", ErrorMessage = "Campaigns can only contain at most 3 words, with a single space between them.")]
        public string CampaignName { get; set; }

        public virtual ICollection<Resident> Residents { get; set; }
    }
}