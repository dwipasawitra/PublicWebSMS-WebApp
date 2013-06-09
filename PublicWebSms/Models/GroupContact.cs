using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // GroupContact: Menyimpan kontak yang ada pada suatu grup
    public class GroupContact
    {
        [Key]
        [Required]
        public int GroupContactId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; }

        [Required]
        public int ContactId { get; set; }

        [ForeignKey("ContactId")]
        public virtual Contact Contact { get; set; }

       
    }
}