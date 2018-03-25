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
using FIVESTARVC.Helpers;
using System.Data.Entity.ModelConfiguration;

namespace FIVESTARVC.Models
{
    public abstract class Person
    {
        protected ResidentContext db = new ResidentContext();

        [Key]
        public int ResidentID { get; set; }
        
        private string LastName { get; set; }

        private string Birthdate { get; set; }

        [Display(Name = "Last Name")]
        [NotMapped]
        public string ClearLastName {

            get {

                return Encryptor.Decrypt(LastName);


            }
                
            set {

                LastName = Encryptor.Encrypt(value);

            }
        }

        [Required]
        [Display(Name = "First Name")]
        public string FirstMidName { get; set; }

        [Display(Name = "Birthdate")]
        [DataType(DataType.Date)]
        [Birthdate(ErrorMessage = "Birthdate must not be in the future.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [NotMapped]
        public DateTime ClearBirthdate {
            get
            {
                return DateTime.Parse(Encryptor.Decrypt(Birthdate.ToString()));

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

        /* return the age of a veteran (in years)S and do not store in the database. */
        public int Age
        {
            get
            {
                try
                {
                    TimeSpan span = DateTime.Now - ClearBirthdate.Date;
                    DateTime age = DateTime.MinValue + span;

                    return age.Year - 1;

                } catch (ArgumentOutOfRangeException /* ex */)
                {
                    return 0;
                }
              
            }
        }

        public string Fullname
        {
            get { return FirstMidName + " " + ClearLastName; }
        }

        /* Remaining days until birthday */
        public int RemainingDays
        {
            get
            {

                DateTime today = DateTime.Today;


                DateTime nextBirthday = ClearBirthdate.AddYears(Age + 1);

                TimeSpan difference = nextBirthday - DateTime.Today;

                return Convert.ToInt32(difference.TotalDays);
            }
        }
        public string BDateMonthName
        {
            get
            {
                CultureInfo ci = new CultureInfo("en-US");
                
                return ClearBirthdate.ToString("MMMM", ci);
                

                
            }
        }

        public class ModelConfiguration : EntityTypeConfiguration<Person>
        {
            public ModelConfiguration()
            {
                Property(p => p.LastName);
                Property(p => p.Birthdate);
            }

        }
    }
}