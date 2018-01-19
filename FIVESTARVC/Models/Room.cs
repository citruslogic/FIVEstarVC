using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public class Room
    {
        [Key, ForeignKey ("Resident")]
        public int RoomID { get; set; }

        public int RoomNum { get; set; }

        public bool IsOccupied { get; set; }

        public virtual Resident Resident { get; set; }

        public static implicit operator Room(int v)
        {
            throw new NotImplementedException();
        }
    }
}