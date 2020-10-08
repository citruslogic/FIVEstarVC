using FIVESTARVC.ViewModels.ResidentDash;
using System.Collections.Generic;

namespace FIVESTARVC.ViewModels
{
    public class MainDashboardData
    {
        public int TotalPopulation { get; set; }

        public int CurrentPopulation { get; set; }

        public int EmergencyShelterCount { get; set; }

        public double Graduated { get; set; }

        public double Admitted { get; set; }

        public double EligibleDischarges { get; set; }

        public string GradPercent { get; set; }

        public List<ResidentBirthdayViewModel> NearestResidents { get; set; }

        public List<ResidentDashData> TopResidents { get; set; }

        public List<ResidentDashData> TopReleasedResidents { get; set; }
    }
}