namespace FIVEstarVC.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Resident_Disability
    {
        
        [Column(Order = 0)]
        public int ResidentID { get; set; }

        [Key]
        [Column(Order = 1)]
        public int DisabilityID { get; set; }

        public int? DisabilityRating { get; set; }

        public virtual Disability Disability { get; set; }

        public virtual Resident Resident { get; set; }
    }
}