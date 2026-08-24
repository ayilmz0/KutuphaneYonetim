namespace KutuphaneYonetim.DTOs
{
    public class KitapDto
    {
        public int KitapId { get; set; }
        public int? KategoriId { get; set; }
        public string KitapAd { get; set; }
        public string Yazar { get; set; }
        public string YayinEvi { get; set; }
        public int SayfaSayisi { get; set; }
        public string ISBN { get; set; }
        public int Stok { get; set; }
        public bool Durum { get; set; }
    }

    public class KitapCreateDto
    {
        public int? KategoriId { get; set; }
        public string KitapAd { get; set; }
        public string Yazar { get; set; }
        public string YayinEvi { get; set; }
        public int SayfaSayisi { get; set; }
        public string ISBN { get; set; }
        public int Stok { get; set; }
        public bool Durum { get; set; }
    }

    public class KitapUpdateDto
    {
        public string KitapAd { get; set; }
        public string Yazar { get; set; }
        public string YayinEvi { get; set; }
        public int SayfaSayisi { get; set; }
        public string ISBN { get; set; }
        public int Stok { get; set; }
        public int? KategoriId { get; set; }
        public bool Durum { get; set; }
    }

    public class KitapDetailDto
    {
        public int KitapId { get; set; }
        public int? KategoriId { get; set; }
        public string KitapAd { get; set; }
        public string Yazar { get; set; }
        public string YayinEvi { get; set; }
        public int SayfaSayisi { get; set; }
        public string ISBN { get; set; }
        public int Stok { get; set; }
        public bool Durum { get; set; }
        public KategoriDto Kategori { get; set; }
    }
}