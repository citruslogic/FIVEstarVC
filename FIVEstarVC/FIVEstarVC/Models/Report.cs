using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVEstarVC.Models
{
    public class Report
    {
        public int reportType { get; set; }

        public int firstName { get; set; }

        public int lastName { get; set; }

        public bool currentResident { get; set; }
    }
}