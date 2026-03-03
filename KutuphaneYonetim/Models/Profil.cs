using System.ComponentModel;

namespace KutuphaneYonetim.Models
{
    public class Profil
    {
        public int ProfilId { get; set; }
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public string Rol { get; set; }
        public bool Durum { get; set; }
    }
}
