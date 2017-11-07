namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Billing")]
    public partial class Billing
    {
        [Key]
        public int ID { get; set; }

        public int PLRecord { get; set; }

        public bool? NotesCompleted { get; set; }

        public bool? CodingCompleted { get; set; }

        public bool? NotesCopied { get; set; }

        public bool? ChargePosted { get; set; }

        public bool? FaceSheetEntered { get; set; }

        public string CPTCodes { get; set; }

        public string POSCodes { get; set; }

        public string MODCodes { get; set; }

        public string DXCodes { get; set; }

        public bool? Purge { get; set; }

        public string viewNotesCompleted()
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
        public string viewNotesCopied()
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

        public string viewFaceSheetEntered()
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

        public string viewCodingCompleted()
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

        public string viewChargePosted()
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

        [DisplayName("Phy Note Completed")]
        public bool checkboxNotesCompleted
        {
            get
            {
                if (NotesCompleted == null || NotesCompleted == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        [DisplayName("Phy Note Copied")]
        public bool checkboxNotesCopied
        {
            get
            {
                if (NotesCopied == null || NotesCopied == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        [DisplayName("Face Sheet Copied")]
        public bool checkboxFaceSheetEntered
        {
            get
            {
                if (FaceSheetEntered == null || FaceSheetEntered == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [DisplayName("Coding Completed")]
        public bool checkboxCodingCompleted
        {
            get
            {
                if (CodingCompleted == null || CodingCompleted == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [DisplayName("Charge Posted")]
        public bool checkboxChargePosted
        {
            get
            {
                if (ChargePosted == null || ChargePosted == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        [DisplayName("Purge Record")]
        [Display(AutoGenerateField = true, Name = "Purge Record")]
        public bool checkboxPurge
        {
            get
            {
                if (Purge == null || Purge == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
