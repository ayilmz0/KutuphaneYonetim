namespace KutuphaneYonetim.DTOs
{
    public class ProfilDto
    {
        public int ProfilId { get; set; }
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public string Rol { get; set; }
        public bool Durum { get; set; }
    }
}