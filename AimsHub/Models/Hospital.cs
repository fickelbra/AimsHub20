namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Hospital")]
    public partial class Hospital
    {
        [Key]
        [StringLength(50)]
        public string ShortName { get; set; }

        [Required]
        public string HospitalName { get; set; }

        [Required]
        [StringLength(50)]
        public string HospitalType { get; set; }

        public bool? Active { get; set; }
    }
}
