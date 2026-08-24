using System;
using System.Linq;
using KutuphaneYonetim.Context;
using KutuphaneYonetim.Models;
using KutuphaneYonetim.DTOs;
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
        public IActionResult Login([FromBody] LoginIstekDto istek)
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

        // Register artık DTO alır ve DTO ile cevap verir
        [HttpPost("Register")]
        public IActionResult Register([FromBody] KullaniciCreateDto dto)
        {
            if (dto == null) return BadRequest(new { success = false, message = "Geçersiz veri." });
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Sifre)) return BadRequest(new { success = false, message = "E-posta ve şifre gerekli." });

            try
            {
                var existing = _context.Kullanici.Any(k => k.Email.ToLower() == dto.Email.ToLower());
                if (existing) return BadRequest(new { success = false, message = "Bu e-posta zaten kayıtlı." });

                var kullanici = new Kullanici
                {
                    Ad = dto.Ad?.Trim(),
                    Soyad = dto.Soyad?.Trim(),
                    Email = dto.Email.Trim(),
                    Rol = dto.Rol ?? "Üye",
                    Sifre = BCrypt.Net.BCrypt.HashPassword(dto.Sifre)
                };

                _context.Kullanici.Add(kullanici);
                _context.SaveChanges();

                if (kullanici.Rol == "Üye")
                {
                    var uye = new Uye
                    {
                        Ad = kullanici.Ad,
                        Soyad = kullanici.Soyad,
                        KayitTarihi = DateTime.Now,
                        Durum = true,
                        KullaniciId = kullanici.KullaniciId
                    };
                    _context.Uye.Add(uye);
                }
                else if (kullanici.Rol == "Personel")
                {
                    var personel = new Personel
                    {
                        PersonelAd = kullanici.Ad,
                        PersonelSoyad = kullanici.Soyad,
                        Durum = true,
                        KullaniciId = kullanici.KullaniciId
                    };
                    _context.Personel.Add(personel);
                }

                _context.SaveChanges();

                var resultDto = new KullaniciDto
                {
                    KullaniciId = kullanici.KullaniciId,
                    Ad = kullanici.Ad,
                    Soyad = kullanici.Soyad,
                    Email = kullanici.Email,
                    Rol = kullanici.Rol
                };

                return Created(string.Empty, resultDto);
            }
            catch (Exception)
            {
                return BadRequest(new { success = false, message = "Kayıt sırasında bir hata oluştu." });
            }
        }
    }

    public class LoginIstekDto
    {
        public string Email { get; set; }
        public string Sifre { get; set; }
    }
}