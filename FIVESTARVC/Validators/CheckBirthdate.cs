using System;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Validators
{
    public class BirthdateAttribute : ValidationAttribute
    {
        public BirthdateAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            var dt = (DateTime)value;
            if (dt <= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}