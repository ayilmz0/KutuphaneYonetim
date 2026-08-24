using KutuphaneYonetim.Context;
using KutuphaneYonetim.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KutuphaneYonetim.Controllers.Api
{
    [Authorize]
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
}