using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.ViewModels
{
    public class ImportedListData
    {
        public List<string> Genders { get; set; }
        public List<string> FirstNames { get; set; }
        public List<string> LastNames { get; set; }
        public List<string> Branches { get; set; }
        public List<string> Arrival { get; set; }
        public List<string> Departure { get; set; }
        public List<string> Notes { get; set; }

    }
}