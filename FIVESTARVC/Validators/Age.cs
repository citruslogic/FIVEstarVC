using FIVESTARVC.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Validators
{
    public class AgeAttribute : ValidationAttribute
    {
        public AgeAttribute()
        {
        }

        public override bool IsValid(object value)
        {
            DateTime? dt = (DateTime?) value;

            if (dt.HasValue)
            {
                 if (DateTime.TryParse(dt.ToString(), out DateTime date))
                {

                    TimeSpan span = DateTime.Now - date.Date;
                    DateTime age = DateTime.MinValue + span;


                    return (age.Year - 1) >= 18;

                }
            }
          

            return false;
        }
    }
}