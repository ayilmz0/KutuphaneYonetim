using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KutuphaneYonetim.Controllers
{
    public class ProfilController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public ProfilController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");
            var token = HttpContext.Session.GetString("JwtToken");

            // Eğer ID veya JWT Token yoksa oturum açılmamıştır, Login'e yönlendir
            if (string.IsNullOrEmpty(kullaniciIdStr) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Kullanici");
            }

            // JWT Token'ı HTTP isteğinin başlığına ekliyoruz
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // API'ye profil verilerini getirmesi için GET isteği atıyoruz
            var response = await _httpClient.GetAsync($"api/Profil/{kullaniciIdStr}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var profil = JsonSerializer.Deserialize<Profil>(jsonString, _jsonOptions);
                return View(profil);
            }

            // Eğer Token süresi dolduysa (401 Unauthorized), oturumu temizleyip yönlendir
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                TempData["Hata"] = "Oturum süreniz doldu, lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Kullanici");
            }

            // Başka bir hata oluşursa boş model dön
            return View(new Profil());
        }
    }
}