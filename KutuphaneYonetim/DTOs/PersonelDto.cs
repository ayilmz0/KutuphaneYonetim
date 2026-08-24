namespace KutuphaneYonetim.DTOs
{
    public class PersonelDto
    {
        public int PersonelId { get; set; }
        public int KullaniciId { get; set; }
        public string PersonelAd { get; set; }
        public string PersonelSoyad { get; set; }
        public bool Durum { get; set; }
    }

    public class PersonelCreateDto
    {
        public int KullaniciId { get; set; }
        public string PersonelAd { get; set; }
        public string PersonelSoyad { get; set; }
        public bool Durum { get; set; }
    }

    public class PersonelUpdateDto
    {
        public string PersonelAd { get; set; }
        public string PersonelSoyad { get; set; }
        public bool Durum { get; set; }
    }
}