using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{

        public enum ReligionType
        {
            [Description("Christianity")]
            [Display(Name = "Christianity")]
            CHRISTIAN,
            [Description("Jewish")]
            [Display(Name = "Jewish")]
            JEWISH,
            [Description("Moslem")]
            [Display(Name = "Moslem")]
            MOSLEM,
            [Description("Buddhist")]
            [Display(Name = "Buddhist")]
            BUDDHIST,
            [Description("Other")]
            [Display(Name = "Other")]
            OTHER,
            [Description("No Affiliation")]
            [Display(Name = "No Affiliation")]
            NO_AF

    }
   
}