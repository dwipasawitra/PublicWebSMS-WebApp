using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Data Model SMS: Menyimpan isi dari suatu SMS
    public class SMS
    {
        [Required]
        [Key]
        public int SmsId { get; set; }

        // SMS Destination: Nomor tujuan pengirim SMS
        [Required]
        [MaxLength(20)]
        public string DestinationNumber { get; set; }

        // SMS Timestamp: Tandai pengiriman SMS dari sistem ini
        [Required]
        public DateTime TimeStamp { get; set; }

        // SMS Content: Isi dari SMS, maksimal 160 karakter
        [MaxLength(160)]
        public string Content { get; set; }

    

        // Boolean untuk penanda apakah SMS sudah dibawa ke sistem SMS gateway
        [Required]
        public bool Sent { get; set; }

    }
}