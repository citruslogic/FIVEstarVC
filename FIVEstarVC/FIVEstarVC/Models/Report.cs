using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVEstarVC.Models
{
    public class Report
    {
        public bool currentResident { get; set; } //True if report is on current resident(s), false if on historical residents

        public bool individual{ get; set; } //True if report on an individual, false if not

        public String firstName { get; set; }

        public String lastName { get; set; }

        public String serviceBranch { get; set; } //Military service branch

        public String campaign { get; set; } //campaign served in

        public int residenceDays { get; set; }

    }
}