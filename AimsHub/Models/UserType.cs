namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserType")]
    public partial class UserType
    {
        [Key]
        [Column("UserType")]
        [StringLength(255)]
        public string UserType1 { get; set; }

        [StringLength(255)]
        public string Description { get; set; }
    }
}
