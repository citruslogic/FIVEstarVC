using FIVESTARVC.DAL;
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

        [Display(Name = "Previous Resident")]
        [StringLength(150)]
        public string LastResident { get; set; }

        [Display(Name = "Days Occupied (current resident)")]
        public int DaysOccupied
        {
            get
            {
                var resident = db.Residents.FirstOrDefault(i => i.Room.RoomNumber == RoomNumber);

                if (resident != null)
                    return resident.DaysInCenter;

                return 0;
            }
        }

        [Display(Name = "Current Resident")]
        public string CurrentResident
        {
            get
            {
                var resident = db.Residents.FirstOrDefault(i => i.Room.RoomNumber == RoomNumber);

                if (resident != null)
                    return resident.Fullname;

                return "No occupant";
            }
        }

        public bool HasLogDetails
        {
            get
            {
                var logs = db.RoomLogs.FirstOrDefault(i => i.RoomNumber == RoomNumber);

                if (logs != null) 
                {
                    return true;
                }

                return false;
            }
        }

    }

}

