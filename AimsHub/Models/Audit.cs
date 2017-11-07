namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Audit")]
    public partial class Audit
    {
        public int ID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }

        [StringLength(50)]
        public string Action { get; set; }

        [StringLength(50)]
        public string Controller { get; set; }

        [StringLength(50)]
        public string Page { get; set; }

        [StringLength(50)]
        public string FunctionUsed { get; set; }

        [StringLength(50)]
        public string TargetIDs { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime TimeStamp { get; set; }
    }
}
