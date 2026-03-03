using Microsoft.EntityFrameworkCore;
using KutuphaneYonetim.Models;

namespace KutuphaneYonetim.Context
{
    public class KutuphaneYonetimContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
        }
        public KutuphaneYonetimContext(DbContextOptions<KutuphaneYonetimContext> options)
            : base(options)
        {
        }
        public DbSet<Kullanici> Kullanici { get; set; }
        public DbSet<Uye> Uye { get; set; }
        public DbSet<Personel> Personel { get; set; }
        public DbSet<Kitap> Kitap { get; set; }
        public DbSet<Kategori> Kategori { get; set; }
        public DbSet<Odunc> Odunc { get; set; }
        public DbSet<Profil> Profil { get; set; }
        public DbSet<OkunanKitaplar> OkunanKitaplar { get; set; }
    }
}
