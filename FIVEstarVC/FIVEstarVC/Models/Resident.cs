namespace FIVEstarVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Resident")]
    public partial class Resident
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Resident()
        {
            Resident_Disability = new HashSet<Resident_Disability>();
            Resident_ProgramEvent = new HashSet<Resident_ProgramEvent>();
            Resident_MilitaryService = new HashSet<Resident_MilitaryService>();
            MilitaryCampaigns = new HashSet<MilitaryCampaign>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ResidentID { get; set; }

        [StringLength(38)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_Disability> Resident_Disability { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_ProgramEvent> Resident_ProgramEvent { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Resident_MilitaryService> Resident_MilitaryService { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MilitaryCampaign> MilitaryCampaigns { get; set; }
    }
}
