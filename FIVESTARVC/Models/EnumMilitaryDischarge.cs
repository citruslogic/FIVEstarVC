using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public enum MilitaryDischargeType
    {
        [Description("Honorable")]
        [Display(Name = "Honorable")]
        HONORABLE,
        [Description("Dishonorable")]
        [Display(Name = "Dishonorable")]
        DISHONORABLE,
        [Description("General Under Honorable")]
        [Display(Name = "General Under Honorable")]
        GENHONORABLE,
        [Description("Other")]
        [Display(Name = "Other")]
        OTHER
    }
}