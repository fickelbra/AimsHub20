using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    public class BillingEditPatient
    {
        [Key]
        public int ID { get; }

        [StringLength(255)]
        [DisplayName("Service Provider")]
        public string Physician { get; }

        [StringLength(255)]
        [DisplayName("Hospital")]
        public string Hospital { get; }

        [StringLength(50)]
        [DisplayName("MRN/FIN")]
        public string MRN_FIN { get; }

        [StringLength(255)]
        [DisplayName("Patient Name")]
        [DisplayFormat(NullDisplayText = "")]
        public string PatientName { get; }

        [DisplayName("Service Type")]
        [StringLength(255)]
        [DefaultValue("Unassigned")]
        public string ServiceType { get; }

        [DisplayName("Service Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:MM/dd/yyyy}", NullDisplayText = "")]
        public DateTime? ServiceDate { get; set; }

        [DisplayName("DOB")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [DisplayName("Gender")]
        [StringLength(10)]
        public string Gender { get; set; }

        public bool? NotesCompleted { get; set; }

        public bool? NotesCopied { get; set; }

        public bool? FaceSheetEntered { get; set; }

        public bool? CodingCompleted { get; set; }

        public bool? ChargePosted { get; set; }

        public bool? Purge { get; set; }

    }
}