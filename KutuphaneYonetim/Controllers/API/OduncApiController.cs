using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.DTOs;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutuphaneYonetim.Controllers.Api
{
    [Authorize]
    [Route("api/Odunc")]
    [ApiController]
    public class OduncApiController : ControllerBase
    {
        private readonly KutuphaneYonetimContext _context;

        public OduncApiController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        // GET: api/Odunc/Uye/5
        [HttpGet("Uye/{uyeId}")]
        public IActionResult GetByUyeId(int uyeId)
        {
            var oduncler = _context.Odunc
                .AsNoTracking()
                .Include(o => o.Kitap)
                    .ThenInclude(k => k.Kategori)
                .Include(o => o.Uye)
                .Where(o => o.UyeId == uyeId && o.Durum == true) // Sadece teslim edilmemişler
                .Select(o => new OduncDetailDto
                {
                    OduncId = o.OduncId,
                    KullaniciId = o.KullaniciId,
                    KitapId = o.KitapId,
                    UyeId = o.UyeId,
                    AlisTarihi = o.AlisTarihi,
                    IadeTarihi = o.IadeTarihi,
                    Ceza = o.Ceza,
                    Durum = o.Durum,
                    Kitap = o.Kitap != null ? new KitapDto
                    {
                        KitapId = o.Kitap.KitapId,
                        KategoriId = o.Kitap.KategoriId,
                        KitapAd = o.Kitap.KitapAd,
                        Yazar = o.Kitap.Yazar,
                        YayinEvi = o.Kitap.YayinEvi,
                        SayfaSayisi = o.Kitap.SayfaSayisi,
                        ISBN = o.Kitap.ISBN,
                        Stok = o.Kitap.Stok,
                        Durum = o.Kitap.Durum
                    } : null,
                    Uye = o.Uye != null ? new UyeDto
                    {
                        UyeId = o.Uye.UyeId,
                        KullaniciId = o.Uye.KullaniciId,
                        Ad = o.Uye.Ad,
                        Soyad = o.Uye.Soyad,
                        KayitTarihi = o.Uye.KayitTarihi,
                        Durum = o.Uye.Durum
                    } : null
                })
                .ToList();

            return Ok(oduncler);
        }

        // POST: api/Odunc/OduncAl
        [HttpPost("OduncAl")]
        public async Task<IActionResult> OduncAl([FromBody] OduncAlDto istekData)
        {
            if (istekData == null || istekData.KitapId <= 0)
            {
                return BadRequest(new { success = false, message = "Geçersiz kitap bilgisi." });
            }

            var kullaniciIdClaim = User.Claims.FirstOrDefault(c => c.Type == "KullaniciId" || c.Type == ClaimTypes.NameIdentifier);

            if (kullaniciIdClaim == null)
            {
                return Unauthorized(new { success = false, message = "Kullanıcı kimliği doğrulanamadı." });
            }

            int kullaniciId = int.Parse(kullaniciIdClaim.Value);

            var uye = await _context.Uye.FirstOrDefaultAsync(u => u.KullaniciId == kullaniciId);
            if (uye == null)
            {
                return BadRequest(new { success = false, message = "Üye kaydınız bulunamadı." });
            }

            var kitap = await _context.Kitap.FirstOrDefaultAsync(k => k.KitapId == istekData.KitapId);
            if (kitap == null || kitap.Stok <= 0)
            {
                return BadRequest(new { success = false, message = "Kitap stokta bulunmamaktadır." });
            }

            var yeniOdunc = new Odunc
            {
                KullaniciId = kullaniciId,
                UyeId = uye.UyeId,
                KitapId = kitap.KitapId,
                AlisTarihi = DateTime.Now,
                IadeTarihi = DateTime.Now.AddDays(15),
                Durum = true,
                Ceza = 0
            };

            kitap.Stok -= 1;

            _context.Odunc.Add(yeniOdunc);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Kitap başarıyla ödünç alındı." });
        }

        [HttpPost("TeslimEt/{id}")]
        public async Task<IActionResult> TeslimEt(int id)
        {
            var odunc = await _context.Odunc.Include(o => o.Kitap).FirstOrDefaultAsync(o => o.OduncId == id);

            if (odunc == null)
            {
                return Ok(new { success = false, message = "Ödünç kaydı bulunamadı." });
            }

            if (!odunc.Durum)
            {
                return Ok(new { success = false, message = "Bu kitap zaten teslim edilmiş." });
            }

            odunc.Durum = false;

            if (odunc.Kitap != null)
            {
                odunc.Kitap.Stok += 1;
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Kitap başarıyla teslim edildi." });
        }
    }
}