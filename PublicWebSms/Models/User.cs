using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // User: Menyimpan data seorang pengguna
    public class User
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Key]
        public string LoginName { get; set; }

        [Required]
        public string LoginPassword { get; set; }
    }
}