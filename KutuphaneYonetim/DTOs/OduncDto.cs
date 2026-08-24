namespace KutuphaneYonetim.DTOs
{
    public class OduncDto
    {
        public int OduncId { get; set; }
        public int KullaniciId { get; set; }
        public int KitapId { get; set; }
        public int UyeId { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public decimal Ceza { get; set; }
        public bool Durum { get; set; }
    }

    public class OduncCreateDto
    {
        public int KullaniciId { get; set; }
        public int KitapId { get; set; }
        public int UyeId { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public bool Durum { get; set; }
    }

    public class OduncUpdateDto
    {
        public DateTime IadeTarihi { get; set; }
        public decimal Ceza { get; set; }
        public bool Durum { get; set; }
    }

    public class OduncDetailDto
    {
        public int OduncId { get; set; }
        public int KullaniciId { get; set; }
        public int KitapId { get; set; }
        public int UyeId { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public decimal Ceza { get; set; }
        public bool Durum { get; set; }
        public KitapDto Kitap { get; set; }
        public UyeDto Uye { get; set; }
    }
}