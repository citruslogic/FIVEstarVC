using FIVESTARVC.Models;
using System.Collections.Generic;

namespace FIVESTARVC.Helpers
{
    public class ProgramEventDateComparer : Comparer<ProgramEvent>
    {

        public override int Compare(ProgramEvent x, ProgramEvent y)
        {
            if (x != null && y != null)
                return x.ClearStartDate.CompareTo(y.ClearStartDate);

            return -1;
        }
    }
}
