namespace FIVESTARVC.ViewModels
{
    public class ResidentDashData
    {
        public int ResidentID { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }
        public int? NumDaysInCenter { get; set; }

        public string LastAdmitDate { get; set; }

        public string LastDischargeDate { get; set; }

    }
}