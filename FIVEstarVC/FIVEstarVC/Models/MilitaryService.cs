namespace FIVEstarVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MilitaryService")]
    public partial class MilitaryService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MilitaryService()
        {
            Resident_MilitaryService = new HashSet<Resident_MilitaryService>();
        }

        [StringLength(120)]
        public string ServiceName { get; set; }

        [StringLength(18)]
        public string MilitaryServiceID { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_MilitaryService> Resident_MilitaryService { get; set; }
    }
}
