using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Models
{
    public class RegisterData
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]

        public bool AcceptTerm { get; set; }
    }
}