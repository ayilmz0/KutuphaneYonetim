namespace KutuphaneYonetim.DTOs
{
    public class OkunanKitaplarDto
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int UyeId { get; set; }
        public int KitapId { get; set; }
        public int KategoriId { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public bool Durum { get; set; }
    }

    public class OkunanKitaplarCreateDto
    {
        public int KullaniciId { get; set; }
        public int UyeId { get; set; }
        public int KitapId { get; set; }
        public int KategoriId { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public bool Durum { get; set; }
    }

    public class OkunanKitaplarDetailDto
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int UyeId { get; set; }
        public int KitapId { get; set; }
        public int KategoriId { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public bool Durum { get; set; }
        public KitapDto Kitap { get; set; }
        public KategoriDto Kategori { get; set; }
    }
}