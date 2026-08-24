using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace KutuphaneYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly KutuphaneYonetimContext _context;

        public HomeController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rol = HttpContext.Session.GetString("Rol");
            var model = new DashboardViewModel();

            if (rol == "Personel")
            {
                model.AktifKullaniciSayisi = _context.Uye.Count(u => u.Durum == true);
                model.ToplamKitapSayisi = _context.Kitap.Count();
                model.ToplamKategoriSayisi = _context.Kategori.Count();
            }
            else if (rol == "Üye")
            {
                // Session'daki tüm olasý ID anahtarlarýný kontrol ediyoruz
                var uyeIdStr = HttpContext.Session.GetString("UyeId");
                var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");

                int.TryParse(uyeIdStr, out int uyeId);
                int.TryParse(kullaniciIdStr, out int kullaniciId);

                // 1. Doðrudan Odunc tablosundan Üyeye ait TÜM kayýtlarý çekiyoruz
                var oduncGecmisi = (from o in _context.Odunc
                                    join k in _context.Kitap on o.KitapId equals k.KitapId
                                    where (uyeId > 0 && o.UyeId == uyeId) || (kullaniciId > 0 && o.KullaniciId == kullaniciId)
                                    select new
                                    {
                                        Id = o.OduncId,
                                        AlisTarihi = (DateTime?)o.AlisTarihi,
                                        IadeTarihi = (DateTime?)o.IadeTarihi,
                                        KitapAd = k.KitapAd,
                                        SayfaSayisi = k.SayfaSayisi // Kitap modelindeki sayfa alaný
                                    }).ToList();

                // 2. Eðer Odunc tablosunda yoksa OkunanKitaplar tablosuna bak
                if (!oduncGecmisi.Any())
                {
                    oduncGecmisi = (from ok in _context.OkunanKitaplar
                                    join k in _context.Kitap on ok.KitapId equals k.KitapId
                                    where (kullaniciId > 0 && ok.KullaniciId == kullaniciId) || (uyeId > 0 && ok.KullaniciId == uyeId)
                                    select new
                                    {
                                        Id = ok.Id,
                                        AlisTarihi = (DateTime?)ok.AlisTarihi,
                                        IadeTarihi = (DateTime?)ok.IadeTarihi,
                                        KitapAd = k.KitapAd,
                                        SayfaSayisi = k.SayfaSayisi
                                    }).ToList();
                }

                // --- HESAPLAMALAR ---

                // Okunan Kitap Sayýsý
                model.OkunanKitapSayisi = oduncGecmisi.Count;

                // Toplam Sayfa Sayýsý (Kitap tablosundaki SayfaSayisi deðerlerini toplar)
                model.ToplamSayfa = oduncGecmisi.Sum(x => x.SayfaSayisi);

                // Son Okunan Kitap Adý (En son tarihli kitabý alýr)
                model.SonOkunanKitapAdi = oduncGecmisi
                    .OrderByDescending(x => x.AlisTarihi)
                    .FirstOrDefault()?.KitapAd ?? "Henüz kitap yok";
            }

            return View(model);
        }

        public IActionResult Anasayfa()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}