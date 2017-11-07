namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BillerPreference
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string UserID { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(255)]
        public string Value { get; set; }
    }
}
