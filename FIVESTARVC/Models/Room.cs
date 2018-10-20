﻿using FIVESTARVC.DAL;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace FIVESTARVC.Models
{

    public class Room
    {
        protected readonly ResidentContext db = new ResidentContext();

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key, Required]
        [Index("RoomNumber")]
        [Display(Name = "Room Number")]
        public int RoomNumber { get; set; }

        [Display(Name = "Occupied?")]
        public bool IsOccupied { get; set; }
        [Display(Name = "Wing")]
        public string WingName { get; set; }

        [Display(Name = "Days Occupied (current resident)")]
        public int? DaysOccupied
        {
            get
            {
                return db.Residents.FirstOrDefault(i => i.Room.RoomNumber == RoomNumber)?.DaysInCenter();
            }
        }

    }

}

