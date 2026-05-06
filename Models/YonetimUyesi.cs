using System.ComponentModel.DataAnnotations;

namespace DernekSitesi.Models
{
    public class YonetimUyesi
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Görev/Unvan alanı zorunludur.")]
        public string Gorev { get; set; } // Örn: Dernek Başkanı, Sayman, Üye vs.

        public string? ResimYolu { get; set; } // Üyenin fotoğrafı

        // Sosyal Medya Linkleri (Zorunlu değil, boş bırakılabilir)
        public string? TwitterLink { get; set; }
        public string? LinkedInLink { get; set; }
        public string? InstagramLink { get; set; }

        public string? Ozgecmis { get; set; }

        public int Siralama { get; set; } // Sitede kimin önce görüneceği (Başkan 1, Yrd 2 vs.)
    }
}