namespace KutuphaneYonetim.Models
{
    public class OkunanKitaplar
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }    
        public int UyeId { get; set; }
        public int KitapId { get; set; }
        public int KategoriId { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public bool Durum { get; set; }
        public Kitap Kitap { get;  set; }
        public Kategori Kategori { get; set; }
    }
}
