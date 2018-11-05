using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.ViewModels
{
    public class DashboardData
    {
        public int ResidentID { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public int? RoomNumber { get; set; }
        public int? NumDaysInCenter { get; set; }

    }
}