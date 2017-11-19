using System.Collections.Generic;

namespace FIVESTARVC.Models
{
    public class ResidentCampaign
    {
        public int ID { get; set; }
        public int ResidentID { get; set; }
        public int MilitaryCampaignID { get; set; }

        public virtual Resident Resident { get; set; }
        public virtual ICollection<MilitaryCampaign> MilitaryCampaign { get; set; }
        


    }
}