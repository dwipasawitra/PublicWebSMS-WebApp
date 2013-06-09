using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    public class UserAproveInput
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public bool Status { get; set; }

    }
}