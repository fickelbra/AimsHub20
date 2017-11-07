namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RefPracSpecialty")]
    public partial class RefPracSpecialty
    {
        public int ID { get; set; }

        public int PracID { get; set; }

        [StringLength(50)]
        public string Specialty { get; set; }

        [StringLength(50)]
        public string FirstChoice { get; set; }

        [StringLength(50)]
        public string Backup { get; set; }

        [StringLength(50)]
        public string Comments { get; set; }
    }
}
