using FIVESTARVC.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;

namespace FIVESTARVC.Validators
{
    [AttributeUsage(AttributeTargets.Class,
    AllowMultiple = true, Inherited = true)]
    public class DischargeDateCheckAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly object _typeId = new object();

        private readonly string _lastAdmittedDate;
        public string ResidentID { get; set; }
        public string DischargeDate { get; set; }

        public DischargeDateCheckAttribute(object obj, string residentID, string dischargeDate)
            : base("The discharge date is not valid.")
        {
            object Object = obj;
            ResidentID = residentID;
            DischargeDate = dischargeDate;

            PropertyDescriptorCollection properties =
           TypeDescriptor.GetProperties(Object);
            int residentIdValue = (int)properties.Find(ResidentID,
                true /* ignoreCase */).GetValue(Object);
            DateTime? dischargeDateValue = (DateTime?)properties.Find(DischargeDate,
                true /* ignoreCase */).GetValue(Object);

            ResidentContext db = new ResidentContext();
                var lastAdmittedDate = db.Residents
                    .Include(i => i.ProgramEvents.Select(j => j.ProgramType))
                    .FirstOrDefault(i => i.ResidentID == residentIdValue)
                    .ProgramEvents
                    .LastOrDefault(i => i.ProgramType.EventType == Models.EnumEventType.ADMISSION)
                    .ClearStartDate;

                _lastAdmittedDate = lastAdmittedDate.ToShortDateString();
        }

        public override object TypeId
        {
            get
            {
                return _typeId;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyDescriptorCollection properties =
           TypeDescriptor.GetProperties(value);
            int residentIdValue = (int)properties.Find(ResidentID,
                true /* ignoreCase */).GetValue(value);
            DateTime? dischargeDateValue = (DateTime?)properties.Find(DischargeDate,
                true /* ignoreCase */).GetValue(value);

            if (residentIdValue != 0)
            {
                
                if (dischargeDateValue < DateTime.Parse(_lastAdmittedDate))
                {
                    return new ValidationResult($"The discharge date {dischargeDateValue} cannot occur before the last admittance date {_lastAdmittedDate}.");
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(metadata.GetDisplayName());
            rule.ValidationParameters.Add("lastadmitted", _lastAdmittedDate);
            rule.ValidationParameters.Add("date", DischargeDate);
            rule.ValidationType = "discharge";
            yield return rule;
        }

    }
}