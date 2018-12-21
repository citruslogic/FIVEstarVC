namespace FIVESTARVC.Models
{
    public class RoomSwapModel
    {
        public Resident FirstResident { get; set; }

        public Resident SecondResident { get; set; }

        public int TempRoom { get; set; }
    }
}