using Microsoft.AspNetCore.Mvc;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using System;
using System.Linq;

namespace KutuphaneYonetim.Controllers
{
    public class ProfilController : Controller
    {
        private readonly KutuphaneYonetimContext _context;

        public ProfilController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");
            if (string.IsNullOrEmpty(kullaniciIdStr) || !int.TryParse(kullaniciIdStr, out int kullaniciId))
            {
                return RedirectToAction("Login", "Kullanici");
            }

            var kullanici = _context.Kullanici.FirstOrDefault(k => k.KullaniciId == kullaniciId);
            if (kullanici == null)
            {
                return View(new Profil
                {
                    AdSoyad = "Bilinmiyor",
                    Email = "Bilinmiyor",
                    Rol = "Bilinmiyor",
                    KayitTarihi = null,
                    Durum = false
                });
            }

            var model = new Profil
            {
                AdSoyad = $"{kullanici.Ad} {kullanici.Soyad}",
                Email = kullanici.Email,
                Rol = kullanici.Rol,
                KayitTarihi = null,
                Durum = false
            };

            if (kullanici.Rol == "Üye")
            {
                var uye = _context.Uye.FirstOrDefault(u => u.KullaniciId == kullanici.KullaniciId);
                if (uye != null)
                {
                    model.KayitTarihi = uye.KayitTarihi;
                    model.Durum = uye.Durum;
                }
            }

            if (kullanici.Rol == "Personel")
            {
                var personel = _context.Personel.FirstOrDefault(u => u.KullaniciId == kullanici.KullaniciId);
                if (personel != null)
                {
                    model.Durum = personel.Durum;
                }
            }
            return View(model);
        }
    }
}
