using Microsoft.EntityFrameworkCore;
using DernekSitesi.Models;

namespace DernekSitesi.Models
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options)
        {
        }

        // Veritabanında oluşacak tablomuz
        public DbSet<Toplanti> Toplantilar { get; set; }
        public DbSet<DernekSitesi.Models.Haber> Haber { get; set; } = default!;
        public DbSet<Uye> Uyeler { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }
        public DbSet<YonetimUyesi> YonetimUyeleri { get; set; }
        public DbSet<GaleriGorsel> GaleriGorseller { get; set; }
    }
}