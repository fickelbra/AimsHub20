namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PatientLogTmp")]
    public partial class PatientLogTmp
    {
        public int ID { get; set; }

        [StringLength(255)]
        public string Physician { get; set; }

        [StringLength(255)]
        public string Hospital { get; set; }

        [StringLength(255)]
        public string PCP_Practice { get; set; }

        [StringLength(50)]
        public string MRN_FIN { get; set; }

        [StringLength(255)]
        public string PatientName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DateCreated { get; set; }

        [StringLength(255)]
        public string ServiceType { get; set; }

        public string Comments { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? LastUpdated { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? ServiceDate { get; set; }

        [StringLength(50)]
        public string RoomNo { get; set; }

        public string AIMSComments { get; set; }

        public string FaceSheet { get; set; }

        public string Notes { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime? DOB { get; set; }

        [StringLength(10)]
        public string Gender { get; set; }

        [StringLength(50)]
        public string PatientClass { get; set; }

        public int PLRecord { get; set; }
    }
}
