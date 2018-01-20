using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FIVESTARVC.Models
{
    public class ProgramEvent
    {
        
        public int ProgramEventID { get; set; }
        [ForeignKey("Resident")]
        public int ResidentID { get; set; }
        [ForeignKey("ProgramType")]
        public int? ProgramTypeID { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        public String Notes { get; set; }
        public Boolean Completed { get; set; }

        public virtual Resident Resident { get; set; }
        public virtual ProgramType ProgramType { get; set; }

    }
}