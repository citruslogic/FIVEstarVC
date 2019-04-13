using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using FIVESTARVC.Helpers;
using FIVESTARVC.Validators;

namespace FIVESTARVC.Models
{
    [CheckTrackDates("ClearStartDate", "ClearEndDate")]
    public class ProgramEvent
    {
        public int ProgramEventID { get; set; }

        [ForeignKey("Resident")]
        public int ResidentID { get; set; }

        [ForeignKey("ProgramType")]
        public int? ProgramTypeID { get; set; }

        [Required]
        private string StartDate { get; set; }

        private string EndDate { get; set; }

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

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [NotMapped]
        public DateTime? ClearEndDate
        {
            get
            {
                if (EndDate != null)
                {
                    
                    return DateTime.Parse(Encryptor.Decrypt(EndDate.ToString()));

                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    EndDate = Encryptor.Encrypt(value.ToString());

                } 

            }
        }
               
        public Boolean Completed { get; set; }

        [NotMapped]
        public bool IsDeleted { get; set; }

        public virtual Resident Resident { get; set; }
        public virtual ProgramType ProgramType { get; set; }

        public String GetShortStartDate()
        {
            return ClearStartDate.ToShortDateString();
        }

        public String GetShortEndDate()
        {
            if (ClearEndDate.HasValue)
            {
                return ClearEndDate.Value.ToShortDateString();

            }
            else {

                return null;
            }

        }

        public String GetLongStartDate()
        {
            return ClearStartDate.ToLongDateString();
        }

        public String GetLongEndDate()
        {
            if (ClearEndDate.HasValue)
            {
                return ClearEndDate.Value.ToLongDateString();

            }
            else
            {

                return null;
            }
        }

        public class ModelConfiguration : EntityTypeConfiguration<ProgramEvent>
        {
            public ModelConfiguration()
            {
                Property(p => p.StartDate);
                Property(p => p.EndDate);
            }
        }
    }
}