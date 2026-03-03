using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutuphaneYonetim.Controllers
{
    public class OduncController : Controller
    {
        private readonly KutuphaneYonetimContext _context;

        public OduncController(KutuphaneYonetimContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var uyeIdStr = HttpContext.Session.GetString("UyeId");
            if (!int.TryParse(uyeIdStr, out int uyeId))
            {
                return View(new List<Odunc>());
            }

            var oduncler = _context.Odunc
                .Include(o => o.Kitap)
                .Where(o => o.UyeId == uyeId)
                .ToList();

            if (!oduncler.Any())
                TempData["Uyari"] = "Henüz ödünç alınmış kitap bulunmamaktadır.";

            return View(oduncler);
        }

        [HttpGet]
        public IActionResult OduncAl(int KitapId)
        {
            var uyeIdStr = HttpContext.Session.GetString("UyeId");
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");

            if (!int.TryParse(uyeIdStr, out int uyeId) || !int.TryParse(kullaniciIdStr, out int kullaniciId))
            {
                TempData["Hata"] = "Kitap ödünç almak için giriş yapmalısınız.";
                return RedirectToAction("Index", "Kitap");
            }

            var kitap = _context.Kitap
                .AsNoTracking()
                .FirstOrDefault(k => k.KitapId == KitapId);

            if (kitap == null)
            {
                TempData["Hata"] = "Kitap bulunamadı.";
                return RedirectToAction("Index", "Kitap");
            }

            if (kitap.Stok <= 0)
            {
                TempData["Hata"] = "Kitabın stoğu kalmamış.";
                return RedirectToAction("Index", "Kitap");
            }

            bool zatenAlindi = _context.Odunc.Any(o => o.KitapId == KitapId && o.UyeId == uyeId && o.Durum == false);
            if (zatenAlindi)
            {
                TempData["Hata"] = "Bu kitabı zaten ödünç aldınız ve henüz teslim etmediniz.";
                return RedirectToAction("Index", "Kitap");
            }

            var odunc = new Odunc
            {
                KitapId = KitapId,
                UyeId = uyeId,
                KullaniciId = kullaniciId,
                AlisTarihi = DateTime.Now,
                IadeTarihi = DateTime.Now.AddDays(15),
                Durum = false
            };

            kitap.Stok -= 1;
            _context.Kitap.Update(kitap);
            _context.Odunc.Add(odunc);
            _context.SaveChanges();

            TempData["Basarili"] = $"Kitap başarıyla ödünç alındı! İade tarihi: {odunc.IadeTarihi:dd.MM.yyyy}";
            return RedirectToAction("Index", "Kitap");
        }

      
        [HttpGet]
        public IActionResult TeslimEt(int id)
        {
            var odunc = _context.Odunc
                .Include(o => o.Uye)   
                .Include(o => o.Kitap) 
                .FirstOrDefault(o => o.OduncId == id);

            if (odunc == null || odunc.Uye == null)
                return Json(new { success = false, message = "Ödünç kaydı veya üye bulunamadı." });

            var kitap = odunc.Kitap;
            if (kitap != null)
                kitap.Stok += 1;

            // Ceza hesaplama
            decimal ceza = odunc.HesaplananCeza;

            var okunan = new OkunanKitaplar
            {
                KullaniciId = odunc.KullaniciId != 0 ? odunc.KullaniciId : odunc.Uye.KullaniciId,
                UyeId = odunc.UyeId,
                KitapId = odunc.KitapId,
                KategoriId = kitap?.KategoriId ?? 0,
                AlisTarihi = odunc.AlisTarihi,
                IadeTarihi = odunc.IadeTarihi,
                Durum = true,
                Kitap = kitap
            };

            _context.OkunanKitaplar.Add(okunan);
            _context.Odunc.Remove(odunc);
            _context.Kitap.Update(kitap);
            _context.SaveChanges();


            string mesaj = ceza > 0
                ? $"Kitap teslim edildi, ancak {ceza}₺ gecikme cezası uygulandı."
                : "Kitap başarıyla teslim edildi!";

            return Json(new { success = true, message = mesaj });
        }
    }
}
