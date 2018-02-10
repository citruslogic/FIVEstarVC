using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{

        public enum ServiceType
        {
            [Description("Air Force")]
            [Display(Name = "Air Force")]
            AIRFORCE,
            [Description("Army")]
            [Display(Name = "Army")]
            ARMY,
            [Description("Coast Guard")]
            [Display(Name = "Coast Guard")]
            COASTGUARD,
            [Description("Marines")]
            [Display(Name = "Marines")]
            MARINES,
            [Description("Navy")]
            [Display(Name = "Navy")]
            NAVY
        }
   
}