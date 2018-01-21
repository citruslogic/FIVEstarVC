using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public class Room
    {
        //internal int ResidentID;

        [Key]
        public int RoomID { get; set; }

        [ForeignKey ("ID")]
        public int ID { get; set; }

        [Display(Name = "Room Number")]
        public int RoomNum { get; set; }

        [Display(Name = "Is Occupied")]
        public bool IsOccupied { get; set; }
        public bool IsSelected { get; set;
        }
        public virtual Resident Resident { get; set; }


        public static implicit operator Room(int v)
        {
            throw new NotImplementedException();
        }
    }
}