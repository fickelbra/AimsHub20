namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserPreference")]
    public partial class UserPreference
    {
        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        public int ID { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [StringLength(50)]
        public string ViewModel { get; set; }

        [StringLength(50)]
        public string FilterName { get; set; }

        [StringLength(50)]
        public string FilterValue { get; set; }
    }
}
