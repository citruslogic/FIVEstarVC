using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using FIVESTARVC.Helpers;

namespace FIVESTARVC.Models
{
    public class ProgramEvent
    {
        
        public int ProgramEventID { get; set; }
        [ForeignKey("Resident")]
        public int ResidentID { get; set; }
        [ForeignKey("ProgramType")]
        public int? ProgramTypeID { get; set; }

        private string StartDate { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [NotMapped]
        public DateTime ClearStartDate {
            get
            {
               
                return DateTime.Parse(Encryptor.Decrypt(StartDate.ToString()));

            }

            set
            {
                StartDate = Encryptor.Encrypt(value.ToString());
            }
        }

       

        public Boolean Completed { get; set; }

        public virtual Resident Resident { get; set; }
        public virtual ProgramType ProgramType { get; set; }
       
        public String GetLongStartDate()
        {
            return ClearStartDate.ToLongDateString();
        }

       

        public class ModelConfiguration : EntityTypeConfiguration<ProgramEvent>
        {
            public ModelConfiguration()
            {
                Property(p => p.StartDate);
            }
        }
    }
}