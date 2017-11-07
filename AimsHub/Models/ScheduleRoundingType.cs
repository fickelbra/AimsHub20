namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScheduleRoundingType")]
    public partial class ScheduleRoundingType
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Event { get; set; }

        public string Description { get; set; }

        [StringLength(50)]
        public string BackColor { get; set; }

        [StringLength(50)]
        public string ForeColor { get; set; }
    }
}
