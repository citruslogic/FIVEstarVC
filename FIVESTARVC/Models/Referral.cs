using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{
    public class Referral
    {
        [Key]
        public int ReferralID { get; set; }

        [Display(Name = "Referral")]
        [StringLength(200, ErrorMessage = "The referral name has a bad length! 200 characters is the maximum allowed. 10 minimum.", MinimumLength = 2)]
        public string ReferralName { get; set; }

        [Display(Name = "Description")]
        [StringLength(200, ErrorMessage = "The optional referral description field is too long. 200 characters is the maximum allowed.")]
        public string AdditionalData { get; set; }

        public virtual ICollection<Resident> Residents { get; set; } = new List<Resident>();
    }
}