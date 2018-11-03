using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{

    public enum EnumEventType
    {
        [Description("Admission")]
        [Display(Name = "Admission")]
        ADMISSION,
        [Description("Discharge")]
        [Display(Name = "Discharge")]
        DISCHARGE,
        [Description("Program")]
        [Display(Name = "Track")]
        TRACK,
        [Description("System")]
        [Display(Name = "Maintenance")]
        SYSTEM
        

    }

}