namespace AimsHub.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RefPracUser")]
    public partial class RefPracUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PracID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserID { get; set; }
    }
}
