using System.ComponentModel.DataAnnotations;

namespace DernekSitesi.Models
{
    public class Uye
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad ve Soyad alanı zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; }

        [Display(Name = "Meslek / Uzmanlık")]
        public string Meslek { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        // Üyenin profil fotoğrafının yolunu tutacağımız sütun
        // Soru işareti (?) koyuyoruz ki fotoğraf yüklemek zorunlu olmasın
        public string? ProfilFotografi { get; set; }

        [Display(Name = "Kan Grubu")]
        public string KanGrubu { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string Telefon { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime KayitTarihi { get; set; } = DateTime.Now; // Otomatik bugünün tarihini atar

        [Display(Name = "Aktif Üye Mi?")]
        public bool AktifMi { get; set; } = true; // Yeni eklenen üye varsayılan olarak aktiftir

        // Sisteme yeni kayıt olan herkes otomatik olarak standart "Uye" yetkisi alır
        public string Yetki { get; set; } = "Uye";

    }
}