namespace FIVEstarVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public enum ProgramNames
    {
        [Description("P2I")]
        P2I,
        [Description("Mental Wellness")]
        MENTALWELLNESS,
        [Description("Work Track")]
        WORKTRACK,
        [Description("School Track")]
        SCHOOLTRACK,
        [Description("Housing Only")]
        HOUSINGONLY
    }

    [Table("ProgramType")]
    public partial class ProgramType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProgramType()
        {
            Resident_ProgramEvent = new HashSet<Resident_ProgramEvent>();
        }

        [Key]
        public int ProgramTypeID { get; set; }

        [StringLength(120)]
        public string ProgramDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_ProgramEvent> Resident_ProgramEvent { get; set; }
    }
}
