using FIVESTARVC.DAL;
using FIVESTARVC.Helpers;
using FIVESTARVC.Validators;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Globalization;

namespace FIVESTARVC.Models
{
    public abstract class Person
    {
        protected readonly ResidentContext db = new ResidentContext();

        [Key]
        public int ResidentID { get; set; }

        private string LastName { get; set; }

        private string FirstMidName { get; set; }

        private string Birthdate { get; set; }

        [Display(Name = "Last Name")]
        [NotMapped]
        public string ClearLastName
        {
            get
            {
                return Encryptor.Decrypt(LastName);
            }

            set
            {
                LastName = Encryptor.Encrypt(value);
            }
        }

        [Required]
        [Display(Name = "First Name")]
        [NotMapped]
        public string ClearFirstMidName
        {
            get
            {
                return Encryptor.Decrypt(FirstMidName);
            }

            set
            {
                FirstMidName = Encryptor.Encrypt(value);
            }
        }

        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [Birthdate(ErrorMessage = "Birthdate must not be in the future.")]
        [Age(ErrorMessage = "Applicant must be 18 years or older.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [NotMapped]
        public DateTime? ClearBirthdate
        {
            get
            {
                if (Birthdate != null)
                {
                    return DateTime.Parse(Encryptor.Decrypt(Birthdate.ToString()));

                }

                return null;
            }

            set
            {
                Birthdate = Encryptor.Encrypt(value.ToString());
            }

        }
               
        [Display(Name = "Gender")]
        public GenderType Gender { get; set; }
        [Display(Name = "Ethnicity")]
        public EthnicityType Ethnicity { get; set; }
        [Display(Name = "Religion")]
        public ReligionType Religion { get; set; }

        [Display(Name = "Home of Record")]
        [ForeignKey("StateTerritory")]
        public int StateTerritoryID { get; set; }
        public virtual StateTerritory StateTerritory { get; set; }

        [Display(Name = "Marked to Delete?")]
        public bool ToDelete { get; set; }

        /* return the age of a veteran (in years)S and do not store in the database. */
        public int Age
        {
            get
            {
                try
                {
                    TimeSpan span = DateTime.Now - ClearBirthdate.GetValueOrDefault().Date;
                    DateTime age = DateTime.MinValue + span;

                    return age.Year - 1;

                }
                catch (ArgumentOutOfRangeException /* ex */)
                {
                    return 0;
                }

            }
        }

        // The last known age of the resident upon release from the center. 
        public int AgeAtRelease { get; set; }

        [Display(Name = "Full Name")]
        public string Fullname
        {
            get { return ClearFirstMidName + " " + ClearLastName; }
        }

        /* Remaining days until birthday */
        public int RemainingDays
        {
            get
            {
                DateTime nextBirthday = ClearBirthdate.GetValueOrDefault().AddYears(Age + 1);

                TimeSpan difference = nextBirthday - DateTime.Today;

                return Convert.ToInt32(difference.TotalDays);
            }
        }

        public string BDateMonthName
        {
            get
            {
                CultureInfo ci = new CultureInfo("en-US");

                return ClearBirthdate?.ToString("MMMM", ci);
            }
        }

        public class ModelConfiguration : EntityTypeConfiguration<Person>
        {
            public ModelConfiguration()
            {
                Property(p => p.FirstMidName);
                Property(p => p.LastName);
                Property(p => p.Birthdate);
            }

        }
    }
}