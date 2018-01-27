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
        /*
         * Type no longer needed in ProgramEvent, so this entity 
         * describes the programs available to a veteran instead,
         * which was the intention.
         */
        public int ProgramTypeID { get; set; }

        [Display(Name = "Program Description")]
        public String ProgramDescription { get; set; }

    }
}