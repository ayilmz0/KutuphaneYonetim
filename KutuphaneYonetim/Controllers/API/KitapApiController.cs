using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using KutuphaneYonetim.DTOs;
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
            var kategorilerDto = kategoriler.Select(k => new KategoriDto
            {
                KategoriId = k.KategoriId,
                KategoriAd = k.KategoriAd
            }).ToList();
            return Ok(kategorilerDto);
        }

        // Tüm Kitaplarý Getir (Index için)
        [HttpGet]
        public IActionResult GetAll()
        {
            var kitaplar = _context.Kitap.Include(k => k.Kategori).ToList();
            var kitaplarDto = kitaplar.Select(k => new KitapDetailDto
            {
                KitapId = k.KitapId,
                KategoriId = k.KategoriId,
                KitapAd = k.KitapAd,
                Yazar = k.Yazar,
                YayinEvi = k.YayinEvi,
                SayfaSayisi = k.SayfaSayisi,
                ISBN = k.ISBN,
                Stok = k.Stok,
                Durum = k.Durum,
                Kategori = k.Kategori != null ? new KategoriDto
                {
                    KategoriId = k.Kategori.KategoriId,
                    KategoriAd = k.Kategori.KategoriAd
                } : null
            }).ToList();
            return Ok(kitaplarDto);
        }

        // Kitap Ekle
        [HttpPost]
        public IActionResult Create([FromForm] KitapCreateDto kitapDto)
        {
            try
            {
                var kitap = new Kitap
                {
                    KategoriId = kitapDto.KategoriId,
                    KitapAd = kitapDto.KitapAd,
                    Yazar = kitapDto.Yazar,
                    YayinEvi = kitapDto.YayinEvi,
                    SayfaSayisi = kitapDto.SayfaSayisi,
                    ISBN = kitapDto.ISBN,
                    Stok = kitapDto.Stok,
                    Durum = kitapDto.Durum
                };
                _context.Add(kitap);
                _context.SaveChanges();
                return Ok(new { success = true, message = "Kitap baþarýyla eklendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Veritabaný hatasý: " + ex.Message });
            }
        }

        // Kitap Güncelle
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromForm] KitapUpdateDto kitapDto)
        {
            var existing = _context.Kitap.Find(id);
            if (existing == null) return NotFound(new { success = false, message = "Kitap bulunamadý." });

            if (!string.IsNullOrEmpty(kitapDto.KitapAd)) existing.KitapAd = kitapDto.KitapAd;
            if (!string.IsNullOrEmpty(kitapDto.Yazar)) existing.Yazar = kitapDto.Yazar;
            if (!string.IsNullOrEmpty(kitapDto.YayinEvi)) existing.YayinEvi = kitapDto.YayinEvi;
            if (kitapDto.SayfaSayisi > 0) existing.SayfaSayisi = kitapDto.SayfaSayisi;
            if (!string.IsNullOrEmpty(kitapDto.ISBN)) existing.ISBN = kitapDto.ISBN;
            if (kitapDto.Stok >= 0) existing.Stok = kitapDto.Stok;
            if (kitapDto.KategoriId != 0) existing.KategoriId = kitapDto.KategoriId;
            existing.Durum = kitapDto.Durum;

            try
            {
                _context.SaveChanges();
                return Ok(new { success = true, message = "Kitap baþarýyla güncellendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Güncelleme hatasý: " + ex.Message });
            }
        }

        // Kitap Sil
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var kitap = _context.Kitap.Find(id);
            if (kitap == null) return NotFound(new { success = false, message = "Kitap bulunamadý." });

            try
            {
                _context.Kitap.Remove(kitap);
                _context.SaveChanges();
                return Ok(new { success = true, message = "Kitap baþarýyla silindi.", KitapId = id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Silme hatasý: " + ex.Message });
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

            var kitaplarDto = kitaplar.Select(k => new KitapDetailDto
            {
                KitapId = k.KitapId,
                KategoriId = k.KategoriId,
                KitapAd = k.KitapAd,
                Yazar = k.Yazar,
                YayinEvi = k.YayinEvi,
                SayfaSayisi = k.SayfaSayisi,
                ISBN = k.ISBN,
                Stok = k.Stok,
                Durum = k.Durum,
                Kategori = k.Kategori != null ? new KategoriDto
                {
                    KategoriId = k.Kategori.KategoriId,
                    KategoriAd = k.Kategori.KategoriAd
                } : null
            }).ToList();

            return Ok(kitaplarDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var kitap = _context.Kitap.Include(k => k.Kategori).FirstOrDefault(k => k.KitapId == id);

            if (kitap == null)
            {
                return NotFound(new { success = false, message = "Kitap bulunamadý." });
            }

            var kitapDto = new KitapDetailDto
            {
                KitapId = kitap.KitapId,
                KategoriId = kitap.KategoriId,
                KitapAd = kitap.KitapAd,
                Yazar = kitap.Yazar,
                YayinEvi = kitap.YayinEvi,
                SayfaSayisi = kitap.SayfaSayisi,
                ISBN = kitap.ISBN,
                Stok = kitap.Stok,
                Durum = kitap.Durum,
                Kategori = kitap.Kategori != null ? new KategoriDto
                {
                    KategoriId = kitap.Kategori.KategoriId,
                    KategoriAd = kitap.Kategori.KategoriAd
                } : null
            };

            return Ok(kitapDto);
        }
    }
}