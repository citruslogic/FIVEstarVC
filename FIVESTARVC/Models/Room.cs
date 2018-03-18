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
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key, Required]
        [Index("RoomNumber")]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        [Display(Name = "Occupied?")]
        public bool IsOccupied { get; set; }
        [Display(Name = "Wing")]
        public string WingName { get; set; }
        
    }
}

