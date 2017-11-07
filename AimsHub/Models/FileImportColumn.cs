using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    [Table("FileImportColumn")]
    public class FileImportColumn
    {
        [Key]
        [StringLength(50)]
        public string Column { get; set; }

        [StringLength(50)]
        public string DisplayName { get; set; }
    }
}