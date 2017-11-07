namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.ComponentModel;

    [Table("PCPCommunication")]
    public partial class PCPCommunication
    {
        public PCPCommunication() { }
        public PCPCommunication(string userID, string docType, string hosp, string patient, int plRecord, string faxNumber, DateTime? dob)
        {
            CommStatus = "Created";
            UserID = userID;
            CommType = "Fax"; //For testing
            DocumentType = docType;
            Hospital = hosp;
            PatientName = patient;
            PLRecord = plRecord;
            DocumentPath = "\\\\AIMSppSE05\\C$\\Data\\Fax\\PatientLog\\" + DocumentType;
            DocumentName = DocumentType + hosp + userID + "_" + PatientName + PLRecord.ToString() + "_" + DateTime.Now.Millisecond.ToString();
            EmailID = "";
            FaxNo = faxNumber;
            DOB = dob;
        }
        public int ID { get; set; }

        [StringLength(255)]
        [DisplayName("Status")]
        public string CommStatus { get; set; }

        [StringLength(255)]
        [DisplayName("UserID")]
        public string UserID { get; set; }

        [StringLength(50)]
        [DisplayName("Type")]
        public string CommType { get; set; }

        [StringLength(255)]
        [DisplayName("DocType")]
        public string DocumentType { get; set; }

        [StringLength(255)]
        [DisplayName("DocName")]
        public string DocumentName { get; set; }

        [StringLength(255)]
        [DisplayName("DocPath")]
        public string DocumentPath { get; set; }

        [StringLength(15)]
        [DisplayName("Fax")]
        public string FaxNo { get; set; }

        [DisplayName("Email")]
        public string EmailID { get; set; }

        [StringLength(255)]
        [DisplayName("ToID")]
        public string ToUserID { get; set; }

        [StringLength(255)]
        [DisplayName("FaxCover")]
        public string FaxCover { get; set; }

        [DisplayName("SchTime")]
        [DataType(DataType.DateTime)]
        public DateTime? ScheduledTime { get; set; }

        [DisplayName("SubTime")]
        [DataType(DataType.DateTime)]
        public DateTime? SubmissionTime { get; set; }

        [DisplayName("StartTime")]
        [DataType(DataType.DateTime)]
        public DateTime? StartTime { get; set; }

        [DisplayName("EndTime")]
        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }

        [DisplayName("Pages")]
        public int? Pages { get; set; }

        [DisplayName("Retries")]
        public int? Retries { get; set; }

        [StringLength(10)]
        [DisplayName("Hosp")]
        public string Hospital { get; set; }

        [StringLength(50)]
        [DisplayName("Patient")]
        public string PatientName { get; set; }

        [DisplayName("Comments")]
        public string Comments { get; set; }

        [DisplayName("PLRecord")]
        public int? PLRecord { get; set; }

        [DisplayName("GenCommHeading")]
        [StringLength(255)]
        public string GenComHeading { get; set; }

        [DisplayName("AddComments")]
        public string AdditionalComments { get; set; }

        [DisplayName("DOB")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
    }
}
