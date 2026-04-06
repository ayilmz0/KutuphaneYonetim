using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutuphaneYonetim.Controllers.API
{
    [Route("api/Kitap")]
    [ApiController]
    public class KitapApiController : ControllerBase
    {
        private readonly KutuphaneYonetimContext _context;

        public KitapApiController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        // Kategori Listesini Getir
        [HttpGet("Kategoriler")]
        public IActionResult GetKategoriler()
        {
            var kategoriler = _context.Kategori.ToList();
            return Ok(kategoriler);
        }

        // Tüm Kitapları Getir (Index için)
        [HttpGet]
        public IActionResult GetAll()
        {
            var kitaplar = _context.Kitap.Include(k => k.Kategori).ToList();
            return Ok(kitaplar);
        }

        // Kitap Ekle
        [HttpPost]
        public IActionResult Create([FromForm] Kitap kitap)
        {
            try
            {
                _context.Add(kitap);
                _context.SaveChanges();
                return Ok(new { success = true, message = "Kitap başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Veritabanı hatası: " + ex.Message });
            }
        }

        // Kitap Güncelle
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] Kitap kitap)
        {
            var existing = _context.Kitap.Find(id);
            if (existing == null) return NotFound(new { success = false, message = "Kitap bulunamadı." });

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
                return Ok(new { success = true, message = "Kitap başarıyla güncellendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Güncelleme hatası: " + ex.Message });
            }
        }

        // Kitap Sil
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var kitap = _context.Kitap.Find(id);
            if (kitap == null) return NotFound(new { success = false, message = "Kitap bulunamadı." });

            try
            {
                _context.Kitap.Remove(kitap);
                _context.SaveChanges();
                return Ok(new { success = true, message = "Kitap başarıyla silindi.", KitapId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Silme hatası: " + ex.Message });
            }
        }

        // Kitap Ara
        [HttpGet("Ara")]
        public IActionResult Ara([FromQuery] string q)
        {
            var kitaplar = _context.Kitap
                .Include(k => k.Kategori)
                .Where(k => string.IsNullOrEmpty(q) || k.KitapAd.Contains(q) || k.Yazar.Contains(q))
                .ToList();

            return Ok(kitaplar);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var kitap = _context.Kitap.Find(id);

            if (kitap == null)
            {
                return NotFound(new { success = false, message = "Kitap bulunamadı." });
            }

            return Ok(kitap);
        }
    }
}