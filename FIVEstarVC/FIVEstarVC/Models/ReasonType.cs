namespace FIVEstarVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public enum DischargeReason
    {

        [Description("Graduated")]
        GRADUATED,
        [Description("Dismissed for Cause")]
        DISMISSED,
        [Description("Self Discharged")]
        SELFDISCHARGED
    }

    [Table("ReasonType")]
    public partial class ReasonType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReasonType()
        {
            Resident_ProgramEvent = new HashSet<Resident_ProgramEvent>();
        }

        [Key]
        public int ReasonID { get; set; }

        [StringLength(50)]
        public string DischargeReason { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_ProgramEvent> Resident_ProgramEvent { get; set; }
    }
}
