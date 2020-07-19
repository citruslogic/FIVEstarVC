using System.Collections.Generic;
using System.Linq;

namespace FIVESTARVC.ViewModels
{
    public class CurrentResidentOverviewViewModel
    {
        public List<CurrentResidentViewModel> CurrentResidents { get; set; }

        public List<IGrouping<CurrentResidentViewModel, CurrentResidentViewModel>> ResidentGroups { get; set; }

        public int AverageAge { get; set; }

        public int ArmyCount { get; set; }

        public int NavyCount { get; set; }

        public int AirForceCount { get; set; }

        public int MarineCount { get; set; }

        public int CoastGuardCount { get; set; }

        public int Post9_11Count { get; set; }

        public int BlackCount { get; set; }

        public int CaucCount { get; set; }

        public int HispCount { get; set; }

        public int AsianCount { get; set; }

        public int NativeCount { get; set; }

        public int OtherCount { get; set; }

        public int Total { get; set; }

        public string TrackType { get; set; }

        public List<CampaignCountViewModel> Campaigns { get; set; }



    }
}