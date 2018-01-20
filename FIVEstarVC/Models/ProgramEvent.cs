using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{
    public class ProgramEvent
    {
        public int ProgramEventID { get; set; }
        public int ResidentID { get; set; }
        public int ProgramTypeID { get; set; }
        
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public String Notes { get; set; }
        public Boolean Completed { get; set; }

        public virtual Resident Resident { get; set; }
        public virtual ProgramType ProgramType { get; set; }

    }
}