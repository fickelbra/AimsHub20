namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("MissingNotesLog")]
    public partial class MissingNotesLog
    {
        [Key]
        public int ID { get; set; }

        public int PLRecord { get; set; }

        public string PhysicianEmail { get; set; }

        [StringLength(50)]
        public string Sender { get; set; }

        public DateTime? SentTime { get; set; }

        public string Comment { get; set; }
    }
}
