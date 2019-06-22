using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.ViewModels
{
    public class ResidentReferralViewModel
    {
        [Display(Name = "Referral")]
        public string ReferralName { get; set; }

        public int CurrentCount { get; set; }
        
        public int CumulCount { get; set; }
    }
}