using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FIVESTARVC.ViewModels
{
    public class AssignRoom
    {
        public int RoomNum { get; set; }


        public bool IsOccupied { get; set; }

        public int ResidentID { get; set; }
    }
}