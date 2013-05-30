using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Kontak yang dimiliki oleh suatu pengguna
    public class ContactUser
    {
        [Key]
        public int ContactUserId { get; set; }

        [Required]
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

        // User yang memiliki kontak ini
        [Required]
        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}