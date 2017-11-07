using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AimsHub.Models
{
    public class AccountChangeModel : AccountModel
    {

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Old Password")]
        public override string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Your New Password")]
        public string newPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Confirm New Password")]
        public string confirmPassword { get; set; }

        [DefaultValue(false)]
        public bool success { get; set; }

        //public string domainPolicy { get; set; }
        //public string lastReset { get; set; }
        //public int daysLeft { get; set; }
    }
}