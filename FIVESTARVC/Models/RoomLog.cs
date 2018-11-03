using System.ComponentModel.DataAnnotations.Schema;

namespace FIVESTARVC.Models
{
    public class RoomLog
    {
        public int RoomLogID { get; set; }

        [ForeignKey("Resident")]
        public int ResidentID { get; set; }

        public Resident Resident { get; set; }

        [ForeignKey("Room")]
        public int? RoomNumber { get; set; }

        public Room Room { get; set; }

        public ProgramEvent Event { get; set; }

        public string Comment { get; set; }
    }
}