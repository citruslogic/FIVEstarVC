using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIVESTARVC.Models;

namespace FIVESTARVC.ViewModels
{
    public class ReportingResidentViewModel
    {
        public int ID { get; set; }

        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime Birthdate { get; set; }
        public Boolean InVetCourt { get; set; }
        public ServiceType ServiceType { get; set; }
        public int Age { get; set; }
        public string Note { get; set; }



        public IEnumerable<int> ProgramTypeID { get; set; }
        public IEnumerable<ProgramEvent> ProgramEvents { get; set; }
        public IEnumerable<DateTime> StartDate { get; set; }



    }

    

}