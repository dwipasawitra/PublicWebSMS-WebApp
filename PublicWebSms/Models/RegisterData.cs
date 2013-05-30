using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PublicWebSms.Models
{
    public class RegisterData
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public bool AcceptTerm { get; set; }
    }
}