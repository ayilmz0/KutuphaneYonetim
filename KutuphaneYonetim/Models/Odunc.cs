using System.ComponentModel.DataAnnotations.Schema;

namespace KutuphaneYonetim.Models
{
    public class Odunc
    {
        public int OduncId { get; set; }
        public int KullaniciId { get; set; }
        public int KitapId { get; set; }
        public Kitap Kitap { get; set; }
        public int UyeId { get; set; }
        public Uye Uye { get; set; }
        public DateTime AlisTarihi { get; set; }
        public DateTime IadeTarihi { get; set; }
        public decimal Ceza { get; set; }
        public bool Durum { get; set; }

        [NotMapped]
        public decimal HesaplananCeza
        {
            get
            {
                if (DateTime.Now <= IadeTarihi)
                    return 0;

                var gecikmeGun = (DateTime.Now - IadeTarihi).TotalDays;
                return (decimal)(Math.Ceiling(gecikmeGun) * 10);
            }
        }
    }
}
