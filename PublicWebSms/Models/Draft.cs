using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Data Model SMS: Menyimpan isi dari suatu SMS
    public class Draft
    {
        [Required]
        [Key]
        public int DraftId { get; set; }

        // SMS Destination: Nomor tujuan pengirim SMS
        [MaxLength(20)]
        public string DestinationNumber { get; set; }

        // SMS Content: Isi dari SMS, maksimal 160 karakter
        [MaxLength(160)]
        public string Content { get; set; }

        // Penanda untuk SMS terjadwal
        [Required]
        public bool Scheduled { get; set; }

        [Required]
        public DateTime ScheduleTime { get; set; }

    }
}