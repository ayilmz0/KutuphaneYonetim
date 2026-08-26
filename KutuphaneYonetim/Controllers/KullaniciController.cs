using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KutuphaneYonetim.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public KullaniciController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Kullanici kullanici)
        {
            // HTML Formundan butonları kaldırdığınız için "Rol" boş gelecek.
            // Modelinizdeki [Required] etiketinin hata vermemesi için Rol kontrolünü siliyoruz:
            ModelState.Remove("Rol");

            if (!ModelState.IsValid)
                return View(kullanici);

            // Güvenlik: MVC tarafında da rolü sabitliyoruz
            kullanici.Rol = "Üye";

            var content = new StringContent(JsonSerializer.Serialize(kullanici, _jsonOptions), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Auth/Register", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Basarili"] = "Kayıt başarıyla tamamlandı. Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            // API'den gelen mesajı parse edip kullanıcıya gösterme (E-posta zaten var vb.)
            var responseString = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseString);
                if (doc.RootElement.TryGetProperty("message", out var messageProp))
                {
                    ModelState.AddModelError("", messageProp.GetString()!);
                }
                else
                {
                    ModelState.AddModelError("", "Kayıt işlemi başarısız oldu.");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Kayıt işlemi başarısız oldu.");
            }

            return View(kullanici);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Register");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Sifre)
        {
            var istek = new { Email = Email, Sifre = Sifre };
            var content = new StringContent(JsonSerializer.Serialize(istek, _jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Auth/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                // API'den dönen verileri Session'a ekle
                if (root.TryGetProperty("token", out var tokenProp))
                    HttpContext.Session.SetString("JwtToken", tokenProp.GetString()!);

                if (root.TryGetProperty("kullaniciId", out var idProp))
                    HttpContext.Session.SetString("KullaniciId", idProp.GetInt32().ToString());

                if (root.TryGetProperty("email", out var emailProp))
                    HttpContext.Session.SetString("Email", emailProp.GetString()!);

                if (root.TryGetProperty("rol", out var rolProp))
                    HttpContext.Session.SetString("Rol", rolProp.GetString()!);

                if (root.TryGetProperty("uyeId", out var uyeIdProp) && uyeIdProp.ValueKind != JsonValueKind.Null)
                    HttpContext.Session.SetString("UyeId", uyeIdProp.GetInt32().ToString());

                if (root.TryGetProperty("personelId", out var perIdProp) && perIdProp.ValueKind != JsonValueKind.Null)
                    HttpContext.Session.SetString("PersonelId", perIdProp.GetInt32().ToString());

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Geçersiz Email veya Şifre.");
            return View("Register");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}