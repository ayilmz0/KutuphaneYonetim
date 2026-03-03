using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace KutuphaneYonetim.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly KutuphaneYonetimContext _context;

        public KullaniciController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                kullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(kullanici.Sifre);

                _context.Kullanici.Add(kullanici);
                _context.SaveChanges();

                HttpContext.Session.SetString("KullaniciId", kullanici.KullaniciId.ToString());
                HttpContext.Session.SetString("Email", kullanici.Email);
                HttpContext.Session.SetString("Rol", kullanici.Rol);

                if (kullanici.Rol == "Üye")
                {
                    Uye uye = new Uye
                    {
                        Ad = kullanici.Ad,
                        Soyad = kullanici.Soyad,
                        KayitTarihi = DateTime.Now,
                        Durum = true,
                        KullaniciId = kullanici.KullaniciId
                    };
                    _context.Uye.Add(uye);
                    _context.SaveChanges();

                    HttpContext.Session.SetString("UyeId", uye.UyeId.ToString());
                }
                else if (kullanici.Rol == "Personel")
                {
                    Personel personel = new Personel
                    {
                        PersonelAd = kullanici.Ad,
                        PersonelSoyad = kullanici.Soyad,
                        Durum = true,
                        KullaniciId = kullanici.KullaniciId
                    };
                    _context.Personel.Add(personel);
                    _context.SaveChanges();

                    HttpContext.Session.SetString("PersonelId", personel.PersonelId.ToString());
                }
                return RedirectToAction("Index", "Home");
            }
            return View(kullanici);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Register");
        }

        [HttpPost]
        public IActionResult Login(string Email, string Sifre)
        {
            var kullanici = _context.Kullanici
                .FirstOrDefault(k => k.Email.ToLower() == Email.ToLower());

            if (kullanici != null && BCrypt.Net.BCrypt.Verify(Sifre, kullanici.Sifre))
            {
                HttpContext.Session.SetString("KullaniciId", kullanici.KullaniciId.ToString());
                HttpContext.Session.SetString("Email", kullanici.Email);
                HttpContext.Session.SetString("Rol", kullanici.Rol);

                if (kullanici.Rol == "Üye")
                {
                    var uye = _context.Uye.FirstOrDefault(u => u.KullaniciId == kullanici.KullaniciId);
                    if (uye != null)
                        HttpContext.Session.SetString("UyeId", uye.UyeId.ToString());
                    else
                        TempData["Hata"] = "Bu kullanıcı için üye kaydı bulunamadı.";
                }
                else if (kullanici.Rol == "Personel")
                {
                    var personel = _context.Personel.FirstOrDefault(p => p.KullaniciId == kullanici.KullaniciId);
                    if (personel != null)
                        HttpContext.Session.SetString("PersonelId", personel.PersonelId.ToString());
                    else
                        TempData["Hata"] = "Bu kullanıcı için personel kaydı bulunamadı.";
                }
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Geçersiz Email veya Şifre.");
            return View("Register");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
