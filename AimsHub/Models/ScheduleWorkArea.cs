namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScheduleWorkArea")]
    public partial class ScheduleWorkArea
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string HospitalShortName { get; set; }

        [StringLength(50)]
        public string ScheduleType { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [StringLength(255)]
        public string Title { get; set; }

        public string RecurrentIDs { get; set; }

        [StringLength(50)]
        public string RecurrentPattern { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string Comments { get; set; }
    }
}
