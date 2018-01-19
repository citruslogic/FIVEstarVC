using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{
    public class ProgramType
    {
        public int ProgramTypeID { get; set; }

        public String ProgramDescription { get; set; }

        public virtual ProgramEvent ProgramEvent { get; set; }
    }
}