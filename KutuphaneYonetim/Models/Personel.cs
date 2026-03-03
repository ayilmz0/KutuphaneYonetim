namespace KutuphaneYonetim.Models
{
    public class Personel
    {
        public int PersonelId { get; set; }
        public int KullaniciId { get; set; }
        public string PersonelAd { get; set; }
        public string PersonelSoyad { get; set; }
        public bool Durum { get; set; }
    }
}
