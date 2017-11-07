namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ScheduleFavorite")]
    public partial class ScheduleFavorite
    {
        public int ID { get; set; }

        public string Users { get; set; }

        public string Hospital { get; set; }

        public string Types { get; set; }

        public bool? Default { get; set; }

        [StringLength(50)]
        public string UserID { get; set; }

        public string Name { get; set; }
    }
}
