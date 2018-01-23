using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FIVESTARVC.Models
{
    public class Room : IEnumerable
    {

        public int RoomID { get; set; }
        public int? ResidentID { get; set; }
        public int RoomNum { get; set; }
        public bool IsOccupied { get; set; }


        public virtual ICollection<Room> Rooms { get; set; }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Rooms).GetEnumerator();
        }
    }
}