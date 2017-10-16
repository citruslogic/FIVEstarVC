namespace FIVEstarVC
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Resident_ProgramEvent
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProgramEventID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProgramTypeID { get; set; }

        [Key]
        [Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ResidentID { get; set; }

        public string Notes { get; set; }

        public bool Completed { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? CreatedID { get; set; }

        public int? ModifiedID { get; set; }

        public virtual ProgramType ProgramType { get; set; }

        public virtual Resident Resident { get; set; }
    }
}
