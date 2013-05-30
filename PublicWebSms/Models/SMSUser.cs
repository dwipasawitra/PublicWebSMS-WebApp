using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Kontak yang dimiliki oleh suatu pengguna
    public class SMSUser
    {
        [Key]
        public int SMSUserId { get; set; }

        [Required]
        public int SMSId { get; set; }

        [ForeignKey("SMSId")]
        public virtual SMS SMS { get; set; }

        // User yang memiliki kontak ini
        [Required]
        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}