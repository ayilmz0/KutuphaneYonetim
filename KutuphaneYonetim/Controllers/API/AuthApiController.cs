using System;
using System.Linq;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KutuphaneYonetim.Controllers.Api
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthApiController : ControllerBase
    {
        private readonly KutuphaneYonetimContext _context;
        private readonly IConfiguration _configuration;

        public AuthApiController(KutuphaneYonetimContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginIstek istek)
        {
            var kullanici = _context.Kullanici.FirstOrDefault(k => k.Email.ToLower() == istek.Email.ToLower());

            if (kullanici == null || !BCrypt.Net.BCrypt.Verify(istek.Sifre, kullanici.Sifre))
            {
                return Unauthorized(new { success = false, message = "Geçersiz E-posta veya Şifre." });
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.KullaniciId.ToString()),
                new Claim(ClaimTypes.Role, kullanici.Rol),
                new Claim(ClaimTypes.Email, kullanici.Email),
                new Claim(ClaimTypes.Name, $"{kullanici.Ad} {kullanici.Soyad}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            // İlişkili ID'leri bul
            int? uyeId = _context.Uye.FirstOrDefault(u => u.KullaniciId == kullanici.KullaniciId)?.UyeId;
            int? personelId = _context.Personel.FirstOrDefault(p => p.KullaniciId == kullanici.KullaniciId)?.PersonelId;

            return Ok(new
            {
                success = true,
                token = new JwtSecurityTokenHandler().WriteToken(token),
                kullaniciId = kullanici.KullaniciId,
                rol = kullanici.Rol,
                email = kullanici.Email,
                uyeId = uyeId,
                personelId = personelId
            });
        }

        [HttpPost("Register")]
        public IActionResult Register([FromBody] Kullanici model)
        {
            if (model == null)
                return BadRequest(new { success = false, message = "Geçersiz veri." });

            try
            {
                var existing = _context.Kullanici.Any(k => k.Email.ToLower() == model.Email.ToLower());
                if (existing)
                    return BadRequest(new { success = false, message = "Bu e-posta zaten kayıtlı." });

                // GÜVENLİK DÜZELTMESİ: İstemciden Rol ne gelirse gelsin sunucuda ezeriz.
                model.Rol = "Üye";
                model.Sifre = BCrypt.Net.BCrypt.HashPassword(model.Sifre); // Şifre güvenliği
                model.Ad = model.Ad?.Trim();
                model.Soyad = model.Soyad?.Trim();
                model.Email = model.Email?.Trim();

                _context.Kullanici.Add(model);
                _context.SaveChanges(); // KullaniciId'nin oluşması için önce kaydediyoruz

                // Gönderdiğiniz Uye modeline uygun olarak Üye tablosuna kayıt
                var uye = new Uye
                {
                    KullaniciId = model.KullaniciId,
                    Ad = model.Ad,
                    Soyad = model.Soyad,
                    KayitTarihi = DateTime.Now,
                    Durum = true
                };

                _context.Uye.Add(uye);
                _context.SaveChanges();

                return Created(string.Empty, new { success = true, message = "Kayıt başarılı." });
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Kayıt sırasında bir hata oluştu." });
            }
        }
    }

    // Login işlemi için basit bir sınıf (Aynı dosyanın en altında kalabilir)
    public class LoginIstek
    {
        public string Email { get; set; }
        public string Sifre { get; set; }
    }
}