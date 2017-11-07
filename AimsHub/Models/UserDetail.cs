namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    [Table("UserDetail")]
    public partial class UserDetail
    {
        [Required]
        [StringLength(255)]
        public string UserType { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string DefaultHospital { get; set; }

        [StringLength(50)]
        public string Title { get; set; }

        public bool? Signed { get; set; }

        public bool Active { get; set; }
    }
}
