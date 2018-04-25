using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.ViewModels
{
    public class ImportedListData
    {
        public List<String> Genders { get; set; }
        public List<String> FirstNames { get; set; }
        public List<String> LastNames { get; set; }
        public List<String> Branches { get; set; }
        public List<String> Arrival { get; set; }
        public List<String> Departure { get; set; }
        public List<String> Notes { get; set; }

    }
}