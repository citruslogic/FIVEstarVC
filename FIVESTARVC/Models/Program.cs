using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{
    public enum ProgramType
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

    public class Program
    {
        public int ID { get; set; }
        public int EventID { get; set; }
        [Display(Name = "Program Completed?")]
        public Boolean HasCompleted { get; set; }

        [Display(Name = "Program")]
        public ProgramType? ResidentProgram { get; set; }

        public Event Event { get; set; }
    }
}