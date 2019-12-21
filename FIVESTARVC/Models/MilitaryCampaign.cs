using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace FIVESTARVC.Models
{

    public class MilitaryCampaign
    {
        public List<MilitaryCampaign> militaryCampaign = new List<MilitaryCampaign>();

        public int MilitaryCampaignID { get; set; }

        [Display(Name = "Campaign")]
        [Required]
        //[RegularExpression("^\\s?([a-zA-Z]+\\s?){1,3}$", ErrorMessage = "Campaigns can only contain at most 3 words, with a single space between them.")]
        [Remote("IsCampaignNameExist", "Residents", AdditionalFields = "MilitaryCampaignID", ErrorMessage = "Campaign name already exists. Choose another or click Close to close the form without submitting.")]
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