using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    public class AccountModel
    {
        [Required]
        [Key]
        [DisplayName("User ID")]
        public string UserID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public virtual string Password { get; set; }
    }
}