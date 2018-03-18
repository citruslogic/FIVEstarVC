using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{

        public enum EthnicityType
        {
            [Description("Caucasian")]
            [Display(Name = "Caucasian")]
            CAUCASIAN,
            [Description("Hispanic or Latino")]
            [Display(Name = "Hispanic / Latino")]
            HISPLATIN,
            [Description("African American")]
            [Display(Name = "African American")]
            AFAM,
            [Description("Native American")]
            [Display(Name = "Native American")]
            NATIVE,
            [Description("Asian / Pacific Islander")]
            [Display(Name = "Asian / Pacific Islander")]
            ASIAN_PACIFIC,
            [Description("Other")]
            [Display(Name = "Other")]
            OTHER

        }
   
}