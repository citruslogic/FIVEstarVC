namespace FIVEstarVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MilitaryCampaign")]
    public partial class MilitaryCampaign
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MilitaryCampaign()
        {
            Residents = new HashSet<Resident>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MilitaryCampaignID { get; set; }

        [StringLength(100)]
        public string Campaign { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident> Residents { get; set; }
    }
}
