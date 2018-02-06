using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIVESTARVC.ViewModels
{
    public class AssignedRoom
    {
        //Items that will be stored when the create resident page is//
        //displayed//
       
        public int RoomNum { get; set; }

        
        public bool IsOccupied { get; set; }

        public int RoomID { get; set; }

        public IEnumerable<SelectListItem> AvailRoom { get; set; }



    }
}