namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("SchedulePattern")]
    public partial class SchedulePattern
    {
        public string Pattern { get; set; }

        public string PatternName { get; set; }

        public int ID { get; set; }
    }
}
