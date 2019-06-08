using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Helpers
{
    internal class Calendar
    {
        /// <summary>
        /// Get months difference between two dates
        /// </summary>
        /// <param name="from">from date</param>
        /// <param name="to">to date</param>
        /// <returns>months (double) between </returns>
        internal static double GetMonths(DateTime from, DateTime to)
        {
            /// |-------X----|---------------|---------------|--X-----------|
            ///         ^                                       ^
            ///       from                                     to

            //change the dates if to is before from
            if (to.Ticks < from.Ticks)
            {
                DateTime temp = from;
                from = to;
                to = temp;
            }

            /// Gets the day percentage of the months = 0...1
            ///
            /// 0            1                               0              1
            /// |-------X----|---------------|---------------|--X-----------|
            /// ^^^^^^^^^                                    ^^^^
            /// percFrom                                    percTo
            double percFrom = (double)from.Day / DateTime.DaysInMonth(from.Year, from.Month);
            double percTo = (double)to.Day / DateTime.DaysInMonth(to.Year, to.Month);

            /// get the amount of months between the two dates based on day one
            /// 
            /// |-------X----|---------------|---------------|--X-----------|
            /// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            ///                        months
            double months = (to.Year * 12 + to.Month) - (from.Year * 12 + from.Month);

            /// Return the right parts
            /// 
            /// |-------X----|---------------|---------------|--X-----------|            
            ///         ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            ///                      return
            return months - percFrom + percTo;
        }
    }
}