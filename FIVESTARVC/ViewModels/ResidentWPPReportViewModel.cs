using FIVESTARVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FIVESTARVC.ViewModels
{
    public class ResidentWPPReportViewModel
    {
        // Month name, WPP Tracks
        public List<IGrouping<string, ResidentWPPTrackAggregate>> ResidentWPPTrackPercentages { get; set; }

        public decimal GrandTotalHours { get; set; }

        public IGrouping<ProgramType, decimal> WPPTotalPercentageByType { get; set; }

        public Dictionary<string, decimal> WPPTotalCountByType { get; set; }
    }

    public class ResidentWPPTrack
    {
        public ProgramEvent Track { get; set; }

        public decimal Percentage { get; set; }

    }

    public class ResidentWPPTrackAggregate
    {
        public string TrackName { get; set; }

        public DateTime StartDate { get; set; }

        public decimal Count { get; set; }

        public decimal Percentage { get; set; }
    }
}