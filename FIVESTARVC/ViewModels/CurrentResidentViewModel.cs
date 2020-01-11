using System;
using System.Collections.Generic;
using FIVESTARVC.Models;

namespace FIVESTARVC.ViewModels
{
    public class CurrentResidentViewModel
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public int? Age { get; set; }

        public string Service { get; set; }

        public string DateAdmitted { get; set; }

        public EthnicityType Ethnicity { get; set; }

        public List<string> Campaigns { get; set; }
    }
}