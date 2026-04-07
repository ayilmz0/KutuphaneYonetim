using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KutuphaneYonetim.Controllers.Api
{
    [Route("api/Odunc")]
    [ApiController]
    public class OduncApiController : ControllerBase
    {
        private readonly KutuphaneYonetimContext _context;

        public OduncApiController(KutuphaneYonetimContext context)
        {
            _context = context;
        }

        // Üyeye göre ödünç alınan kitapları getir
        [HttpGet("Uye/{uyeId}")]
        public IActionResult GetByUyeId(int uyeId)
        {
            var oduncler = _context.Odunc
                .Include(o => o.Kitap)
                .Where(o => o.UyeId == uyeId)
                .ToList();

            return Ok(oduncler);
        }

        // Kitap Ödünç Al
        [HttpPost("OduncAl")]
        public IActionResult OduncAl([FromBody] OduncIstekDto istek)
        {
            var kitap = _context.Kitap.FirstOrDefault(k => k.KitapId == istek.KitapId);

            if (kitap == null)
                return NotFound(new { success = false, message = "Kitap bulunamadı." });

            if (kitap.Stok <= 0)
                return BadRequest(new { success = false, message = "Kitabın stoğu kalmamış." });

            bool zatenAlindi = _context.Odunc.Any(o => o.KitapId == istek.KitapId && o.UyeId == istek.UyeId && o.Durum == false);
            if (zatenAlindi)
                return BadRequest(new { success = false, message = "Bu kitabı zaten ödünç aldınız ve henüz teslim etmediniz." });

            var odunc = new Odunc
            {
                KitapId = istek.KitapId,
                UyeId = istek.UyeId,
                KullaniciId = istek.KullaniciId,
                AlisTarihi = DateTime.Now,
                IadeTarihi = DateTime.Now.AddDays(15),
                Durum = false
            };

            kitap.Stok -= 1;
            _context.Kitap.Update(kitap);
            _context.Odunc.Add(odunc);
            _context.SaveChanges();

            return Ok(new { success = true, message = $"Kitap başarıyla ödünç alındı! İade tarihi: {odunc.IadeTarihi:dd.MM.yyyy}" });
        }

        // Kitap Teslim Et
        [HttpPost("TeslimEt/{id}")]
        public IActionResult TeslimEt(int id)
        {
            var odunc = _context.Odunc
                .Include(o => o.Uye)
                .Include(o => o.Kitap)
                .FirstOrDefault(o => o.OduncId == id);

            if (odunc == null || odunc.Uye == null)
                return NotFound(new { success = false, message = "Ödünç kaydı veya üye bulunamadı." });

            var kitap = odunc.Kitap;
            if (kitap != null)
                kitap.Stok += 1;

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

            return Ok(new { success = true, message = mesaj });
        }
    }

    // API'ye veri göndermek için kullanacağımız basit bir model
    public class OduncIstekDto
    {
        public int KitapId { get; set; }
        public int UyeId { get; set; }
        public int KullaniciId { get; set; }
    }
}