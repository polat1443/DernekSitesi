namespace DernekSitesi.Models
{
    public class Toplanti
    {
        public int Id { get; set; } // Veritabanı için benzersiz kimlik (Primary Key)
        public string Baslik { get; set; }
        public DateTime Tarih { get; set; }
        public string Konum { get; set; }
        public bool GelecekToplantiMi { get; set; } // Etiket için true/false
    }
}