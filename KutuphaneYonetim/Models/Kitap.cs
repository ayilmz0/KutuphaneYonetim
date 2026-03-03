using System.ComponentModel.DataAnnotations;

namespace KutuphaneYonetim.Models
{
    public class Kitap
    {
        public int KitapId { get; set; }
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        public int? KategoriId { get; set; }
        public string KitapAd { get; set; }
        public string Yazar { get; set; }
        public string YayinEvi { get; set; }
        public int SayfaSayisi { get; set; }
        public string ISBN { get; set; }
        public int Stok { get; set; }
        public bool Durum { get; set; }
        public Kategori Kategori { get; set; }  
    }
}
