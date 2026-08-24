using System.Linq;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.DTOs;
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

        // Üyeye göre ödünç alınan kitapları DTO ile döndür
        [HttpGet("Uye/{uyeId}")]
        public IActionResult GetByUyeId(int uyeId)
        {
            var oduncler = _context.Odunc
                .AsNoTracking()
                .Include(o => o.Kitap)
                    .ThenInclude(k => k.Kategori)
                .Include(o => o.Uye)
                .Where(o => o.UyeId == uyeId)
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

        // (Diğer endpoint'ler olduğu gibi bırakıldı...)
    }
}