using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Kontak yang dimiliki oleh suatu pengguna
    public class DraftUser
    {
        [Key]
        public int DraftUserId { get; set; }

        [Required]
        public int DraftId { get; set; }

        [ForeignKey("DraftId")]
        public virtual Draft Draft { get; set; }

        // User yang memiliki kontak ini
        [Required]
        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}