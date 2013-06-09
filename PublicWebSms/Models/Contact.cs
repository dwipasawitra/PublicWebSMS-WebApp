using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PublicWebSms.Models
{
    // Data Model Contact: Menyimpan sebuah item kontak
    public class Contact
    {
        [Required]
        [Key]
        public int ContactId { get; set; }

        // Nama: Isi nama dari kontak
        [Required]
        public string Nama { get; set; }

        // Nomor kontak: Isi nomor dari kontak
        [Required]
        public string Nomor { get; set; }
    }
}