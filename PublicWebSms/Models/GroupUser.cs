using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Kontak yang dimiliki oleh suatu pengguna
    public class GroupUser
    {
        [Key]
        public int GroupUserId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        // User yang memiliki group ini
        [Required]
        public string UserName { get; set; }

        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}