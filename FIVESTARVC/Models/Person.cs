using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using FIVESTARVC.DAL;
using FIVESTARVC.Validators;
using DelegateDecompiler;
using System.Globalization;

namespace FIVESTARVC.Models
{
    public abstract class Person
    {
        protected ResidentContext db = new ResidentContext();

        [Key]
        public int ResidentID { get; set; }
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [Birthdate(ErrorMessage = "Birthdate must not be in the future.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? Birthdate { get; set; }

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

        /* return the age of a veteran (in years)S and do not store in the database. */
        public int Age
        {
            get
            {
                TimeSpan span = DateTime.Now - Birthdate.GetValueOrDefault(DateTime.Now);
                DateTime age = DateTime.MinValue + span;

                return age.Year - 1;
            }
        }

        public string Fullname
        {
            get { return FirstMidName + " " + LastName; }
        }

        /* Remaining days until birthday */
        public int RemainingDays
        {
            get
            {

                DateTime today = DateTime.Today;
                if (Birthdate.HasValue)
                {
                    DateTime nextBirthday = Birthdate.Value.AddYears(Age + 1);

                    TimeSpan difference = nextBirthday - DateTime.Today;

                    return Convert.ToInt32(difference.TotalDays);
                }

                return 0;
            }
        }
        public string BDateMonthName
        {
            get
            {
                CultureInfo ci = new CultureInfo("en-US");
                if (Birthdate.HasValue)
                {
                    return Birthdate.Value.ToString("MMMM", ci);
                }

                return null;
            }
        }
    }
}