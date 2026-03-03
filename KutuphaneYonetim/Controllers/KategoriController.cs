using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace KutuphaneYonetim.Controllers
{
    public class KategoriController : Controller
    {
        private readonly KutuphaneYonetimContext _context;
        public KategoriController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var kategoriler = _context.Kategori.ToList();
            return View(kategoriler);
        }

        public IActionResult Create()
        {
            ViewBag.Kategori = new SelectList(_context.Kategori.ToList(), "KategoriId", "KategoriAd");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateAjax([Bind("KategoriId,KategoriAd")] Kategori kategori)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Hatalı giriş yapıldı." });
            }

            bool kategoriVarMi = _context.Kategori
                .Any(k => k.KategoriAd.ToLower() == kategori.KategoriAd.ToLower());

            if (kategoriVarMi)
            {
                return Json(new { success = false, message = "Kategori zaten var." });
            }
            _context.Kategori.Add(kategori);
            _context.SaveChanges();

            return Json(new { success = true, message = "Kategori başarıyla eklendi." });
        }

        [HttpPost]
        public IActionResult DeleteAjax(int id)
        {
            var kategori = _context.Kategori.FirstOrDefault(m => m.KategoriId == id);
            if (kategori == null)
            {
                return Json(new { success = false, message = "Kategori bulunamadı." });
            }

            // Önce o kategoriye bağlı kitapları sil
            var kitaplar = _context.Kitap.Where(k => k.KategoriId == id).ToList();
            if (kitaplar.Any())
            {
                _context.Kitap.RemoveRange(kitaplar);
            }

            // Sonra kategoriyi sil
            _context.Kategori.Remove(kategori);
            _context.SaveChanges();

            return Json(new { success = true, message = "Kategori ve bağlı kitaplar silindi.", kategoriId = id });
        }

        [HttpGet]
        public IActionResult AraAjax(string q)
        {
            var kategoriler = string.IsNullOrEmpty(q) ? _context.Kategori.ToList() : _context.Kategori
                    .Where(k => k.KategoriAd.Contains(q)).ToList();

            return PartialView("_KategoriTable", kategoriler);
        }

    }
}
