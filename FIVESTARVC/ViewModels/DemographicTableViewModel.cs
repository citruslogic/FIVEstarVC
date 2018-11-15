using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.ViewModels
{
    public class DemographicTableViewModel
    {
        public bool IsCurrent { get; set; }

        public virtual List<GenderGroup> GenderComposition { get; set; }

        public virtual List<AgeGroups> AgeComposition { get; set; }
    }
}