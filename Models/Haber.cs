using System.ComponentModel.DataAnnotations;

namespace DernekSitesi.Models
{
    public class Haber
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Haber başlığı boş bırakılamaz.")]
        [Display(Name = "Haber Başlığı")]
        public string Baslik { get; set; }

        [Display(Name = "Haber İçeriği")]
        public string Icerik { get; set; }

        [Display(Name = "Yayınlanma Tarihi")]
        public DateTime YayinTarihi { get; set; } = DateTime.Now;

        [Display(Name = "Önemli mi?")]
        public bool IsPriority { get; set; } // Ana sayfada en üstte çıksın mı?

        public string? ResimYolu { get; set; }
        // Soru işareti (?) koyduk ki resmi olmayan eski haberler hata vermesin, boş kalabilsin.
    }
}