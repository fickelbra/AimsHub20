using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    [Table("FaxServiceType")]
    public class FaxServiceType
    {
        [Key]
        [StringLength(50)]
        public string Service { get; set; }

        [StringLength(50)]
        public string FaxType { get; set; }
    }
}