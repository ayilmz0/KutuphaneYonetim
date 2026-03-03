namespace KutuphaneYonetim.Models
{
    public class DashboardViewModel
    {
        public int OkunanKitapSayisi { get; set; }
        public int ToplamSayfa { get; set; }
        public string SonOkunanKitapAdi { get; set; }

        public int AktifKullaniciSayisi { get; set; }
        public int ToplamKitapSayisi { get; set; }
        public int ToplamKategoriSayisi { get; set; }
    }
}
