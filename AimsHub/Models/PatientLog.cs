namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    [Table("PatientLog")]
    public partial class PatientLog
    {
        [Key]
        public int ID { get; set; }

        [StringLength(255)]
        [DisplayName("Physician")]
        [DisplayFormat(NullDisplayText = "Unassigned")]
        public string Physician { get; set; }

        [StringLength(255)]
        [DisplayName("Hospital")]
        public string Hospital { get; set; }

        [StringLength(255)]
        [DisplayName("PCP")]
        [DisplayFormat(NullDisplayText = "No PCP")]
        public string PCP_Practice { get; set; }

        [StringLength(50)]
        [DisplayName("MRN_FIN")]
        public string MRN_FIN { get; set; }

        [StringLength(255)]
        [DisplayName("Patient")]
        [DisplayFormat(NullDisplayText = "")]
        public string PatientName { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }

        [DisplayName("ServiceType")]
        [StringLength(255)]
        [DefaultValue("Unassigned")]
        public string ServiceType { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:MM/dd/yyyy}", NullDisplayText = "")]
        public DateTime? LastUpdated { get; set; }

        [DisplayName("ServiceDate")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:MM/dd/yyyy}", NullDisplayText = "")]
        public DateTime? ServiceDate { get; set; }

        [StringLength(50)]
        [DisplayName("Room")]
        [DisplayFormat(NullDisplayText = "")]
        public string RoomNo { get; set; }

        [DisplayName("AIMSComments")]
        [DisplayFormat(NullDisplayText = "")]
        public string AIMSComments { get; set; }

        [DisplayName("Facesheet")]
        [DisplayFormat(NullDisplayText = "")]
        public string FaceSheet { get; set; }

        [DisplayName("Notes")]
        [DisplayFormat(NullDisplayText = "")]
        public string Notes { get; set; }

        [DisplayName("DOB")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true, NullDisplayText = "")]
        public DateTime? DOB { get; set; }

        [DisplayName("Gender")]
        [StringLength(10)]
        [DefaultValue("Male")]
        public string Gender { get; set; }

        [DisplayName("PtClass")]
        [StringLength(50)]
        [DefaultValue("None")]
        public string PatientClass { get; set; }

    }
}
