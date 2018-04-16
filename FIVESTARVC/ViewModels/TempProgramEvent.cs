using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using FIVESTARVC.Helpers;
using System.Web.Mvc;
using FIVESTARVC.Models;

namespace FIVESTARVC.ViewModels
{
    public class TempProgramEvent
    {
        public TempProgramEvent()
        {
            StartDate = DateTime.Now;
        }
        
        public int ProgramEventID { get; set; }
        [ForeignKey("Resident")]
        public int ResidentID { get; set; }
        [ForeignKey("ProgramType")]
        public int? ProgramTypeID { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EndDate { get; set; }

        public Boolean Completed { get; set; }

        public virtual Resident Resident { get; set; }
        public virtual ProgramType ProgramType { get; set; }

        

       
    }
}