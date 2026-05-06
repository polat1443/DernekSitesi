using System;
using System.ComponentModel.DataAnnotations;

namespace DernekSitesi.Models
{
    public class Mesaj
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen adınızı yazın")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Konu boş bırakılamaz")]
        public string Konu { get; set; }

        [Required(ErrorMessage = "Mesajınızı yazın")]
        public string Icerik { get; set; }

        public DateTime GonderimTarihi { get; set; } = DateTime.Now;
        public bool OkunduMu { get; set; } = false;
    }
}