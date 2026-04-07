using KutuphaneYonetim.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KutuphaneYonetim.Controllers.Api
{
    [Authorize] // DİKKAT: Bu kilit sayesinde Token'ı olmayan kimse bu API'ye ulaşamaz!
    [Route("api/Profil")]
    [ApiController]
    public class ProfilApiController : ControllerBase
    {
        private readonly KutuphaneYonetimContext _context;

        public ProfilApiController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult GetProfil(int id)
        {
            var kullanici = _context.Kullanici.FirstOrDefault(k => k.KullaniciId == id);

            if (kullanici == null)
                return NotFound();

            // Sadece gerekli verileri taşıyan DTO'muzu hazırlıyoruz
            var profilDto = new ProfilDto
            {
                AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}",
                Email = kullanici.Email,
                Rol = kullanici.Rol,
                Durum = false
            };

            if (kullanici.Rol == "Üye")
            {
                var uye = _context.Uye.FirstOrDefault(u => u.KullaniciId == kullanici.KullaniciId);
                if (uye != null)
                {
                    profilDto.KayitTarihi = uye.KayitTarihi;
                    profilDto.Durum = uye.Durum;
                }
            }
            else if (kullanici.Rol == "Personel")
            {
                var personel = _context.Personel.FirstOrDefault(p => p.KullaniciId == kullanici.KullaniciId);
                if (personel != null)
                {
                    profilDto.Durum = personel.Durum;
                }
            }

            return Ok(profilDto);
        }
    }

    // API'den MVC'ye döneceğimiz veri paketi
    public class ProfilDto
    {
        public string AdSoyad { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }
        public DateTime? KayitTarihi { get; set; }
        public bool Durum { get; set; }
    }
}