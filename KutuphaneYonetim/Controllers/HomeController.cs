using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;

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
                var uyeIdStr = HttpContext.Session.GetString("KullaniciId");
                if (string.IsNullOrEmpty(uyeIdStr) || !int.TryParse(uyeIdStr, out int uyeId))
                {
                    return RedirectToAction("Register", "Kullanici");
                }

                var userOkunanKitaplar = (from ok in _context.OkunanKitaplar
                                          join k in _context.Kitap on ok.KitapId equals k.KitapId
                                          where ok.KullaniciId == uyeId
                                          select new
                                          {
                                              ok.Id,
                                              ok.AlisTarihi,
                                              ok.IadeTarihi,
                                              k.KitapAd,
                                              k.SayfaSayisi
                                          }).ToList();

                model.OkunanKitapSayisi = userOkunanKitaplar.Count;
                model.ToplamSayfa = userOkunanKitaplar.Sum(x => x.SayfaSayisi);
                model.SonOkunanKitapAdi = userOkunanKitaplar
                         .Where(x => x.IadeTarihi != null)
                         .OrderByDescending(x => x.IadeTarihi)
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
