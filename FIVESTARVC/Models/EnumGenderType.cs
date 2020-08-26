using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{

    public enum GenderType
        {
            [Description("Male")]
            [Display(Name = "Male")]
            MALE,
            [Description("Female")]
            [Display(Name = "Female")]
            FEMALE,
            [Description("LGBTQ")]
            [Display(Name = "LGBTQ")]
            LGBT

        }
   
}