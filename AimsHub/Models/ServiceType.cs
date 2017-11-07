namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ServiceType")]
    public partial class ServiceType
    {
        [Key]
        [StringLength(50)]
        public string Service { get; set; }

        public string Description { get; set; }
    }
}
