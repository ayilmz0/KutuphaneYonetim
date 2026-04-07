using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace KutuphaneYonetim.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly HttpClient _httpClient;

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
            if (!ModelState.IsValid)
                return View(kullanici);

            var content = new StringContent(JsonSerializer.Serialize(kullanici), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/Auth/Register", content);

            if (response.IsSuccessStatusCode)
            {
                // Kayıt başarılıysa direkt login sayfasına (veya Home'a) yönlendir
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Kayıt işlemi başarısız oldu.");
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
            var content = new StringContent(JsonSerializer.Serialize(istek), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Auth/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(jsonString);

                // API'den gelen verileri alıp Session'a koyuyoruz
                HttpContext.Session.SetString("JwtToken", result.GetProperty("token").GetString());
                HttpContext.Session.SetString("KullaniciId", result.GetProperty("kullaniciId").GetInt32().ToString());
                HttpContext.Session.SetString("Email", result.GetProperty("email").GetString());
                HttpContext.Session.SetString("Rol", result.GetProperty("rol").GetString());

                // Null gelme ihtimaline karşı kontrol (UyeId ve PersonelId)
                if (result.TryGetProperty("uyeId", out var uyeIdProp) && uyeIdProp.ValueKind != JsonValueKind.Null)
                    HttpContext.Session.SetString("UyeId", uyeIdProp.GetInt32().ToString());

                if (result.TryGetProperty("personelId", out var perIdProp) && perIdProp.ValueKind != JsonValueKind.Null)
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