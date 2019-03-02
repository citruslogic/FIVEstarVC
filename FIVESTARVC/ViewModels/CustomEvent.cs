using FIVESTARVC.Models;
using System.Collections.Generic;

namespace FIVESTARVC.ViewModels
{
    public class CustomEvent
    {
        //TODO: change "Event" to "Track" at a later date.
        public List<TempProgramEvent> ProgramEvents { get; set; } = new List<TempProgramEvent>();
        public List<TempProgramEvent> EnrolledTracks { get; set; } = new List<TempProgramEvent>(); 
    }
}