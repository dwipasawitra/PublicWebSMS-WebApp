using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        [Key]
        [Required]
        public string AdminName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}