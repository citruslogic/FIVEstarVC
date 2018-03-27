using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
            [Description("LGBT")]
            [Display(Name = "LGBT")]
            LGBT

        }
   
}