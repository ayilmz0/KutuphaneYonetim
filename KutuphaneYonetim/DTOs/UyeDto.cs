namespace KutuphaneYonetim.DTOs
{
    public class UyeDto
    {
        public int UyeId { get; set; }
        public int KullaniciId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool Durum { get; set; }
    }

    public class UyeCreateDto
    {
        public int KullaniciId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public bool Durum { get; set; }
    }

    public class UyeUpdateDto
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public bool Durum { get; set; }
    }

    public class UyeDetailDto
    {
        public int UyeId { get; set; }
        public int KullaniciId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool Durum { get; set; }
        public KullaniciDto Kullanici { get; set; }
    }
}