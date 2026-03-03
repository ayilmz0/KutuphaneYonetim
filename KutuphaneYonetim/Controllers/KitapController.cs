using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KutuphaneYonetim.Controllers
{
    public class KitapController : Controller
    {
        private readonly KutuphaneYonetimContext _context;
        public KitapController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var kitaplar = _context.Kitap
                .Include(k => k.Kategori)
                .ToList();
            return View(kitaplar);
        }

        public IActionResult Create()
        {
            ViewBag.Kategori = new SelectList(_context.Kategori.ToList(), "KategoriId", "KategoriAd");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAjax([Bind("KitapId,KategoriId,KitapAd,Yazar,YayinEvi,SayfaSayisi,ISBN,Stok,Durum")] Kitap kitap)
        {
            try
            {
                _context.Add(kitap);
                _context.SaveChanges();
                return Json(new { success = true, message = "Kitap başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Veritabanı hatası: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult DeleteAjax(int id)
        {
            var kitap = _context.Kitap.FirstOrDefault(k => k.KitapId == id);

            if (kitap == null)
            {
                return Json(new { success = false, message = "Kitap bulunamadı." });
            }
            try
            {
                _context.Kitap.Remove(kitap);
                _context.SaveChanges();
                return Json(new { success = true, message = "Kitap başarıyla silindi.", KitapId = id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Silme hatası: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var kitap = _context.Kitap.Find(id);
            if (kitap == null) return NotFound();

            ViewBag.Kategori = new SelectList(_context.Kategori.ToList(), "KategoriId", "KategoriAd", kitap.KategoriId);
            return View(kitap);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAjax([Bind("KitapId,KategoriId,KitapAd,Yazar,YayinEvi,SayfaSayisi,ISBN,Stok,Durum")] Kitap kitap)
        {
            var existing = _context.Kitap.Find(kitap.KitapId);
            if (existing == null) return Json(new { success = false, message = "Kitap bulunamadı." });

            if (!string.IsNullOrEmpty(kitap.KitapAd)) existing.KitapAd = kitap.KitapAd;
            if (!string.IsNullOrEmpty(kitap.Yazar)) existing.Yazar = kitap.Yazar;
            if (!string.IsNullOrEmpty(kitap.YayinEvi)) existing.YayinEvi = kitap.YayinEvi;
            if (kitap.SayfaSayisi > 0) existing.SayfaSayisi = kitap.SayfaSayisi;
            if (!string.IsNullOrEmpty(kitap.ISBN)) existing.ISBN = kitap.ISBN;
            if (kitap.Stok >= 0) existing.Stok = kitap.Stok;
            if (kitap.KategoriId != 0) existing.KategoriId = kitap.KategoriId;
            existing.Durum = kitap.Durum;

            try
            {
                _context.SaveChanges();
                return Json(new { success = true, message = "Kitap başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Güncelleme hatası: " + ex.Message });
            }
        }

        private bool KitapExists(int id)
        {
            return _context.Kitap.Any(e => e.KitapId == id);
        }

        [HttpGet]
        public IActionResult AraAjax(string q)
        {
            var kitaplar = _context.Kitap
            .Include(k => k.Kategori)
            .Where(k => string.IsNullOrEmpty(q) || k.KitapAd.Contains(q) || k.Yazar.Contains(q))
            .ToList();

            return PartialView("_KitapTable", kitaplar);
        }
    }
}
