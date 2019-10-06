using FIVESTARVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace FIVESTARVC.Helpers
{
    public static class PersonComputations
    {
        private class LastNameCalculation : Computation<Person, string>
        {
            public override Expression<Func<ComputedInput<TSource, Person>, ComputedOutput<TSource, string>>> GetComputation<TSource>()
            {
                return input => new ComputedOutput<TSource, string>
                {
                    Source = input.Source,  // This just passes through our source object to the output projection
                    Value = input.Item.ClearLastName
                };
            }
        }
        private class FirstMidNameCalculation : Computation<Person, string>
        {
            public override Expression<Func<ComputedInput<TSource, Person>, ComputedOutput<TSource, string>>> GetComputation<TSource>()
            {
                return input => new ComputedOutput<TSource, string>
                {
                    Source = input.Source,  // This just passes through our source object to the output projection
                    Value = input.Item.ClearFirstMidName
                };
            }
        }

        public static Computation<Person, string> LastName = new LastNameCalculation();
        public static Computation<Person, string> FirstMidName = new FirstMidNameCalculation();

    }
}