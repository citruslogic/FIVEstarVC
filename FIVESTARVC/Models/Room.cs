using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FIVESTARVC.Models
{
   
    public class Room
    {
        public List<Room> room = new List<Room>();

        public int RoomID { get; set; }

        [Display(Name = "Room Number")]
        public int RoomNum { get; set; }

        [Display(Name = "Is Occupied")]
        public bool IsOccupied { get; set; }
        [Display(Name = "Wing")]
        public string WingName { get; set; }

        public IEnumerable<SelectListItem> RoomIEnum
        {
            get
            {
                return new SelectList(room, "RoomID", "RoomNum");

            }

        }

    }
}

