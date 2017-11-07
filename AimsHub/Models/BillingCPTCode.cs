namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BillingCPTCode")]
    public partial class BillingCPTCode
    {
        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public string Description { get; set; }      
    }
}
