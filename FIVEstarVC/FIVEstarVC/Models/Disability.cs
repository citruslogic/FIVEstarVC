namespace FIVEstarVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Disability")]
    public partial class Disability
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Disability()
        {
            Resident_Disability = new HashSet<Resident_Disability>();
        }

        [Key]
        public int DisabilityID { get; set; }

        [StringLength(50)]
        public string Condition { get; set; }

        [Column(TypeName = "money")]
        public decimal? BenefitAmount { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_Disability> Resident_Disability { get; set; }
    }
}
