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

        [Display(Name = "Description")]
        public String ProgramDescription { get; set; }

    }
}