using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FIVESTARVC.Models;

namespace FIVESTARVC.ViewModels
{
    public class HistoricData
    {
        public Resident resident = new Resident();
        public ProgramEvent program = new ProgramEvent();

        public int totalAdmitted { get; set; }

    }
}