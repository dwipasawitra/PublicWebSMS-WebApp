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
        public int UserId { get; set; }

        [Required]
        public bool Status { get; set; }

    }
}