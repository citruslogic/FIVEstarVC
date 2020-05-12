using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FIVESTARVC.Models
{

    public enum ServiceType
        {
            [Description("Air Force")]
            [Display(Name = "Air Force")]
            AIRFORCE,
            [Description("Army")]
            [Display(Name = "Army")]
            ARMY,
            [Description("Coast Guard")]
            [Display(Name = "Coast Guard")]
            COASTGUARD,
            [Description("Marines")]
            [Display(Name = "Marines")]
            MARINES,
            [Description("Navy")]
            [Display(Name = "Navy")]
            NAVY,
            [Description("National Guard")]
            [Display(Name = "NG")]
            NG,
            [Description("Reserves")]
            [Display(Name = "Reserves")]
            RESERVE
        }

    public enum NGReserveServiceType
    {
        [Description("Air Force")]
        [Display(Name = "Air Force")]
        AIRFORCE = 1,
        [Description("Army")]
        [Display(Name = "Army")]
        ARMY = 2,
        [Description("Coast Guard")]
        [Display(Name = "Coast Guard")]
        COASTGUARD = 3,
        [Description("Marines")]
        [Display(Name = "Marines")]
        MARINES = 4,
        [Description("Navy")]
        [Display(Name = "Navy")]
        NAVY = 5
    }
   
}