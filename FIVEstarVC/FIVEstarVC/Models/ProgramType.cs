namespace FIVEstarVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProgramType")]
    public partial class ProgramType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProgramType()
        {
            Resident_ProgramEvent = new HashSet<Resident_ProgramEvent>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProgramTypeID { get; set; }

        [StringLength(120)]
        public string ProgramDescription { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_ProgramEvent> Resident_ProgramEvent { get; set; }
    }
}
