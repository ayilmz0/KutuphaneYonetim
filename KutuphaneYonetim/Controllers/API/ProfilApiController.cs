using System.Linq;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
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
            // 1. Ana Kullanıcı tablosundan veriyi bul
            var kullanici = _context.Kullanici.FirstOrDefault(k => k.KullaniciId == id);

            if (kullanici == null)
                return NotFound(new { success = false, message = "Kullanıcı bulunamadı." });

            // 2. Profil model nesnesini varsayılan bilgilerle oluştur
            var profil = new Profil
            {
                ProfilId = kullanici.KullaniciId,
                Email = kullanici.Email,
                Rol = kullanici.Rol,
                AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}".Trim(),
                Durum = false
            };

            // 3. Kullanıcının Rolüne göre ilişkili tablodan detay verileri doldur
            if (kullanici.Rol == "Üye")
            {
                var uye = _context.Uye.FirstOrDefault(u => u.KullaniciId == kullanici.KullaniciId);
                if (uye != null)
                {
                    profil.AdSoyad = $"{uye.Ad} {uye.Soyad}".Trim();
                    profil.KayitTarihi = uye.KayitTarihi;
                    profil.Durum = uye.Durum;
                }
            }
            else if (kullanici.Rol == "Personel")
            {
                var personel = _context.Personel.FirstOrDefault(p => p.KullaniciId == kullanici.KullaniciId);
                if (personel != null)
                {
                    profil.AdSoyad = $"{personel.PersonelAd} {personel.PersonelSoyad}".Trim();
                    profil.Durum = personel.Durum;
                }
            }

            return Ok(profil);
        }
    }
}