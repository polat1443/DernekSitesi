using System;
using System.ComponentModel.DataAnnotations;

namespace DernekSitesi.Models
{
    public class GaleriGorsel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen albüm için bir başlık girin.")]
        [Display(Name = "Albüm Başlığı")]
        public string Baslik { get; set; }

        public string? ResimYolu { get; set; } // Bu artık KAPAK FOTOĞRAFI

        // YENİ: İçeriğe eklenecek toplu fotoğrafların yollarını tutacak
        public string? CokluFotograflar { get; set; }

        [Display(Name = "Eklenme Tarihi")]
        public DateTime EklenmeTarihi { get; set; } = DateTime.Now;
    }
}