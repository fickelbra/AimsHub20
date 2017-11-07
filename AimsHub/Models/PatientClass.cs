using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    [Table("PatientClass")]
    public class PatientClass
    {

        [Key]
        [StringLength(50)]
        public string ShortName { get; set; }

        [StringLength(50)]
        public string Class { get; set; }
    }
}