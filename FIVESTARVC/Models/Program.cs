using System;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{
    public enum ProgramType
    {
        [Display(Name = "Veterans Court")]
        VETCOURT,
        [Display(Name = "Finance Education")]
        FINANCIAL,
        [Display(Name = "Mental Wellness")]
        MENTAL_WELLNESS,
        [Display(Name = "Work Program")]
        WORK_PROGRAM,
        [Display(Name = "School Program")]
        SCHOOL_PROGRAM,
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