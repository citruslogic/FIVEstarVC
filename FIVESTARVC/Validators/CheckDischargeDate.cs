using FIVESTARVC.DAL;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Data.Entity;
using FIVESTARVC.Models;

namespace FIVESTARVC.Validators
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class DischargeDateCheckAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage = "The discharge date you have entered is invalid.";

        private readonly object _typeId = new object();

        public int ResidentID { get; private set; }
        public DateTime DischargeDate { get; private set; }

        public DischargeDateCheckAttribute(int residentID, DateTime? date)
        {
            ResidentID = residentID;
            DischargeDate = date.GetValueOrDefault();
        }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentUICulture, ErrorMessageString,
                ResidentID, DischargeDate);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ResidentContext db = new ResidentContext();

            var residentTrackEvent = db.Residents
             .Include(p => p.ProgramEvents)
             .Where(r => r.ResidentID == ResidentID)
             .Single().ProgramEvents.LastOrDefault(j => j.ProgramType.EventType == EnumEventType.ADMISSION);

            if (residentTrackEvent.ClearStartDate < DischargeDate)
            {
                return new ValidationResult("The discharge date cannot be before last admission date " + residentTrackEvent.ClearStartDate);

            }

            return ValidationResult.Success;
        }
    }
}
