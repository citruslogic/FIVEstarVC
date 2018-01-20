using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{
    public enum ResidentProgramType
    {
        [Description("Veterans Court")]
        [Display(Name = "Veterans Court")]
        VETCOURT,
        [Description("Financial Education")]
        [Display(Name = "Finance Education")]
        FINANCIAL,
        [Description("Mental Wellness")]
        [Display(Name = "Mental Wellness")]
        MENTAL_WELLNESS,
        [Description("Work Program")]
        [Display(Name = "Work Program")]
        WORK_PROGRAM,
        [Description("School Program")]
        [Display(Name = "School Program")]
        SCHOOL_PROGRAM,
        [Description("Emergency Shelter")]
        [Display(Name = "Emergency Shelter")]
        EM_SHELTER
    }

    public class ProgramType
    {
        public int ProgramTypeID { get; set; }
        public int ProgramEventID { get; set; }

        public ResidentProgramType ResidentProgramType { get; set; }
        public String ProgramDescription { get; set; }

        public virtual ProgramEvent ProgramEvent { get; set; }
    }
}