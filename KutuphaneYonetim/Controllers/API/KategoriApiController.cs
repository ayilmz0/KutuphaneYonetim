using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutuphaneYonetim.Controllers.Api
{
   
    [Route("api/Kategori")]
    [ApiController]
    public class KategoriApiController : ControllerBase
    {
        private readonly KutuphaneYonetimContext _context;

        public KategoriApiController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        // Tüm Kategorileri Getir
        [HttpGet]
        public IActionResult GetAll()
        {
            var kategoriler = _context.Kategori.ToList();
            return Ok(kategoriler);
        }

        // Kategori Ekle
        [HttpPost]
        public IActionResult Create([FromForm] Kategori kategori)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Hatalı giriş yapıldı." });
            }

            // Aynı isimde kategori var mı kontrolü
            bool kategoriVarMi = _context.Kategori
                .Any(k => k.KategoriAd.ToLower() == kategori.KategoriAd.ToLower());

            if (kategoriVarMi)
            {
                return BadRequest(new { success = false, message = "Kategori zaten var." });
            }

            try
            {
                _context.Kategori.Add(kategori);
                _context.SaveChanges();
                return Ok(new { success = true, message = "Kategori başarıyla eklendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Veritabanı hatası: " + ex.Message });
            }
        }

        // Kategori ve Bağlı Kitapları Sil
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var kategori = _context.Kategori.FirstOrDefault(m => m.KategoriId == id);

            if (kategori == null)
            {
                return NotFound(new { success = false, message = "Kategori bulunamadı." });
            }

            try
            {
                // Önce o kategoriye bağlı kitapları sil (Cascading Delete)
                var kitaplar = _context.Kitap.Where(k => k.KategoriId == id).ToList();
                if (kitaplar.Any())
                {
                    _context.Kitap.RemoveRange(kitaplar);
                }

                // Sonra kategoriyi sil
                _context.Kategori.Remove(kategori);
                _context.SaveChanges();

                return Ok(new { success = true, message = "Kategori ve bağlı kitaplar başarıyla silindi.", kategoriId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Silme hatası: " + ex.Message });
            }
        }

        // Kategori Ara
        [HttpGet("Ara")]
        public IActionResult Ara([FromQuery] string q)
        {
            var kategoriler = string.IsNullOrEmpty(q)
                ? _context.Kategori.ToList()
                : _context.Kategori.Where(k => k.KategoriAd.Contains(q)).ToList();

            return Ok(kategoriler);
        }
    }
}