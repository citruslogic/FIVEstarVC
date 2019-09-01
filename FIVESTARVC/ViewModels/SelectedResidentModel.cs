using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.ViewModels
{
    public class SelectedResidentModel
    {
        public bool ToDelete { get; set; }

        public bool ToRestore { get; set; }

        public int ResidentID { get; set; }

        public string Fullname { get; set; }
    }
}