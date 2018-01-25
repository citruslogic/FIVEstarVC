﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public class Room
    {

        public int RoomID { get; set; }

        public int? ResidentID { get; set; }

        [Display(Name = "Room Number")]
        public int RoomNum { get; set; }

        [Display(Name = "Is Occupied")]
        public bool IsOccupied { get; set; }

    }
}
