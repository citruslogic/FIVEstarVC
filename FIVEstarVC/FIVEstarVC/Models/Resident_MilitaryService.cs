namespace FIVEstarVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Resident_MilitaryService
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(60)]
        public string ResidentMilitaryServiceID { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(18)]
        public string MilitaryServiceID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ResidentID { get; set; }

        public virtual MilitaryService MilitaryService { get; set; }

        public virtual Resident Resident { get; set; }
    }
}
