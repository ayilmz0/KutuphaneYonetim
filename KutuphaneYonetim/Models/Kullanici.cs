using System.ComponentModel.DataAnnotations;

namespace KutuphaneYonetim.Models
{
    public class Kullanici
    {
        public int KullaniciId { get; set; }

        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Email alanı zorunludur.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        public string Sifre { get; set; }

        [Required(ErrorMessage = "Rol alanı zorunludur.")]
        public string Rol { get; set; }
    }
}
