using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FIVESTARVC.Validators
{
    [AttributeUsage(AttributeTargets.Class,
    AllowMultiple = true, Inherited = true)]
    public sealed class CheckTrackDatesAttribute : ValidationAttribute
    {
        private const string _defaultErrorMessage =
            "The track start date you have entered is invalid because it either comes before 2011 or is set later than the current calendar year. Please check the date and resubmit.";

        private readonly object _typeId = new object();
        

        public CheckTrackDatesAttribute(string clearStartDate, string clearEndDate) : base(_defaultErrorMessage)
        {
            ClearStartDate = clearStartDate;
            ClearEndDate = clearEndDate;
        }

        public string ClearStartDate { get; private set; }
        public string ClearEndDate { get; private set; }

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
                ClearStartDate, ClearEndDate);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyDescriptorCollection properties =
           TypeDescriptor.GetProperties(value);
            DateTime startDateValue = (DateTime) properties.Find(ClearStartDate,
                true /* ignoreCase */).GetValue(value);
            DateTime? endDateValue = (DateTime?) properties.Find(ClearEndDate,
                true /* ignoreCase */).GetValue(value);

            if (startDateValue >= DateTime.Now.AddYears(1) || startDateValue.Year < 2011)
            {
                return new ValidationResult(_defaultErrorMessage);

            } else if (endDateValue.HasValue && endDateValue.Value.Year < 2011)
            {
                return new ValidationResult("The end date year for a track cannot be before 2011");

            } else if (endDateValue.HasValue && endDateValue.Value >= DateTime.Now.AddYears(1))
            {
                return new ValidationResult("The end date year for a track cannot be a year ahead of " + DateTime.Now.Year);
            }
            else if (endDateValue.HasValue && !string.Equals(DateTime.Today.ToShortTimeString(), endDateValue.Value.ToShortDateString(), StringComparison.InvariantCulture) && endDateValue.Value < startDateValue)
            {
                return new ValidationResult("The end date cannot come before the start date.");
            }
                        
            return ValidationResult.Success;
        }

    }
}