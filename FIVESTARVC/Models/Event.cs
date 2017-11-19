using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{


    public class Event
    {
        public int ID { get; set; }
        public int ResidentID { get; set; }

        [Display(Name = "Date Admitted")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AdmitDate { get; set; }
        [Display(Name = "Date Discharged")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LeaveDate { get; set; }
        [Display(Name = "Notes")]
        public string Notes { get; set; }
        
        public virtual ICollection<Program> Programs { get; set; }

        public virtual Resident Resident { get; set; }
        
    }
}