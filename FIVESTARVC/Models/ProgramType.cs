using System;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{

    public class ProgramType
    {
        [Key]
        public int ProgramTypeID { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "The new track name is too long. " +
            "Maximum length of the name is 250 characters (letters and numbers).")]
        public String ProgramDescription { get; set; }

        [Display(Name = "Event Type")]
        public EnumEventType EventType { get; set; }

    }
}