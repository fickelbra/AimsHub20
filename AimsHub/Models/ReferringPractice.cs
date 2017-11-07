namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ReferringPractice")]
    public partial class ReferringPractice
    {
        [Key]
        public int PracID { get; set; }

        [Required]
        [StringLength(255)]
        public string PracName { get; set; }

        [StringLength(255)]
        public string Address1 { get; set; }

        [StringLength(255)]
        public string Address2 { get; set; }

        [StringLength(255)]
        public string Address3 { get; set; }

        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Zip { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

        [StringLength(50)]
        public string Fax { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(50)]
        public string OfficeManager { get; set; }

        [Column(TypeName = "ntext")]
        public string Other { get; set; }

        [StringLength(30)]
        public string PDFPassword { get; set; }

        public bool EmailNotification { get; set; }

        public bool FaxNotification { get; set; }

        [StringLength(50)]
        public string LegacyShortName { get; set; }

        public string viewEmailNotification
        {
            get
            {
                if (EmailNotification == true)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }
        public string viewFaxNotification
        {
            get
            {
                if (FaxNotification == true)
                {
                    return "Yes";
                }
                else
                {
                    return "No";
                }
            }
        }
    }
}
