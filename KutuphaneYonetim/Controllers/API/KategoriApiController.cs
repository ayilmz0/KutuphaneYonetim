using System;
using System.Linq;
using System.Collections.Generic;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using KutuphaneYonetim.DTOs;
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

        // Tüm Kategorileri Getir (DTO)
        [HttpGet]
        public IActionResult GetAll()
        {
            var kategoriler = _context.Kategori.AsNoTracking().ToList();
            var dto = kategoriler.Select(k => new KategoriDto
            {
                KategoriId = k.KategoriId,
                KategoriAd = k.KategoriAd
            }).ToList();
            return Ok(dto);
        }

        // Kategori Ekle (DTO alır)
        [HttpPost]
        public IActionResult Create([FromBody] KategoriDto kategoriDto)
        {
            if (kategoriDto == null || string.IsNullOrWhiteSpace(kategoriDto.KategoriAd))
                return BadRequest(new { success = false, message = "Geçersiz veri." });

            // Aynı isimde kategori var mı kontrolü (case-insensitive)
            bool kategoriVarMi = _context.Kategori
                .Any(k => k.KategoriAd != null && k.KategoriAd.ToLower() == kategoriDto.KategoriAd.Trim().ToLower());

            if (kategoriVarMi)
            {
                return BadRequest(new { success = false, message = "Kategori zaten var." });
            }

            try
            {
                var kategori = new Kategori
                {
                    KategoriAd = kategoriDto.KategoriAd.Trim()
                };

                _context.Kategori.Add(kategori);
                _context.SaveChanges();

                var createdDto = new KategoriDto
                {
                    KategoriId = kategori.KategoriId,
                    KategoriAd = kategori.KategoriAd
                };

                return CreatedAtAction(nameof(GetAll), createdDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Veritabanı hatası: " + ex.Message });
            }
        }

        // Kategori ve Bağlı Kitapları Sil (DTO response)
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
                // Önce o kategoriye bağlı kitapları sil (eğer cascade yoksa)
                var kitaplar = _context.Kitap.Where(k => k.KategoriId == id).ToList();
                if (kitaplar.Any())
                {
                    _context.Kitap.RemoveRange(kitaplar);
                }

                // Sonra kategoriyi sil
                _context.Kategori.Remove(kategori);
                _context.SaveChanges();

                var dto = new KategoriDto
                {
                    KategoriId = kategori.KategoriId,
                    KategoriAd = kategori.KategoriAd
                };

                return Ok(new { success = true, message = "Kategori ve bağlı kitaplar başarıyla silindi.", kategori = dto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Silme hatası: " + ex.Message });
            }
        }

        // Kategori Ara (DTO)
        [HttpGet("Ara")]
        public IActionResult Ara([FromQuery] string q)
        {
            var sorgu = _context.Kategori.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                sorgu = sorgu.Where(k => k.KategoriAd != null && EF.Functions.Like(k.KategoriAd, $"%{q}%"));
            }

            var kategoriler = sorgu.AsNoTracking().ToList();

            var dto = kategoriler.Select(k => new KategoriDto
            {
                KategoriId = k.KategoriId,
                KategoriAd = k.KategoriAd
            }).ToList();

            return Ok(dto);
        }
    }
}