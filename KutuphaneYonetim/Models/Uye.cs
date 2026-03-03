namespace KutuphaneYonetim.Models
{
    public class Uye
    {
        public int UyeId { get; set; }
        public int KullaniciId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime KayitTarihi { get; set; }
        public bool Durum { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}
