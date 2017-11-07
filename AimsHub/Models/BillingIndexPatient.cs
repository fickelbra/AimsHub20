using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    public class BillingIndexPatient
    {
        [Key]
        public int ID { get; set; }

        public int PLRecord { get; set; }

        [StringLength(255)]
        [DisplayName("Physician")]
        [DefaultValue("Unassigned")]
        public string Physician { get; set; }

        [StringLength(255)]
        [DisplayName("Hosp")]
        public string Hospital { get; set; }

        [StringLength(255)]
        [DisplayName("PCP")]
        public string PCP_Practice { get; set; }

        [StringLength(50)]
        [DisplayName("MRN")]
        public string MRN_FIN { get; set; }

        [StringLength(255)]
        [DisplayName("Patient")]
        [DisplayFormat(NullDisplayText = "")]
        public string PatientName { get; set; }

        [DisplayName("ServiceType")]
        [StringLength(255)]
        [DefaultValue("Unassigned")]
        public string ServiceType { get; set; }

        [DisplayName("ServiceDate")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, ConvertEmptyStringToNull = true, DataFormatString = "{0:MM/dd/yyyy}", NullDisplayText = "")]
        public DateTime? ServiceDate { get; set; }

        [DisplayName("DOB")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }

        [DisplayName("Gender")]
        [StringLength(10)]
        [DefaultValue("Male")]
        public string Gender { get; set; }

        public bool? NotesCompleted { get; set; }

        public bool? NotesCopied { get; set; }

        public bool? FaceSheetEntered { get; set; }

        public bool? CodingCompleted { get; set; }

        public bool? ChargePosted { get; set; }

        public bool? Purge { get; set; }

        public string viewNotesCompleted
        {
            get
            {
                if (NotesCompleted == null || NotesCompleted == false)
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
        }

        public string viewNotesCopied
        {
            get
            {
                if (NotesCopied == null || NotesCopied == false)
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
        }

        public string viewFaceSheetEntered
        {
            get
            {
                if (FaceSheetEntered == null || FaceSheetEntered == false)
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
        }

        public string viewCodingCompleted
        {
            get
            {
                if (CodingCompleted == null || CodingCompleted == false)
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
        }

        public string viewChargePosted
        {
            get
            {
                if (ChargePosted == null || ChargePosted == false)
                {
                    return "No";
                }
                else
                {
                    return "Yes";
                }
            }
        }
    }
}