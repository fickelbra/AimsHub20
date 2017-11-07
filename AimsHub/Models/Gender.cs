namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    [Table("Gender")]
    public partial class Gender
    {
        [Key]
        [Column("Gender", Order = 0)]
        [StringLength(50)]
        [DisplayName("Gender")]
        public string Gender1 { get; set; }

        [Key]
        [Column(Order = 1)]
        [DisplayName("Gender")]
        [StringLength(50)]
        public string ShortName { get; set; }
    }
}
