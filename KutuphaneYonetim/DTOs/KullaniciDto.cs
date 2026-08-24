namespace KutuphaneYonetim.DTOs
{
    public class KullaniciDto
    {
        public int KullaniciId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }

    public class KullaniciCreateDto
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public string Rol { get; set; }
    }

    public class KullaniciUpdateDto
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
    }
}