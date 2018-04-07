using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIVESTARVC.Models
{


    public class ProgramType
    {
        public int ProgramTypeID { get; set; }

        [Required]
        [Display(Name = "Description")]
        [MaxLength(250, ErrorMessage = "The new track name is too long. " +
            "Maximum length of the name is 250 characters (letters and numbers).")]
        public String ProgramDescription { get; set; }

    }
}