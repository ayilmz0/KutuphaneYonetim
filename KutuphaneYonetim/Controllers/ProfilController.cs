using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace KutuphaneYonetim.Controllers
{
    public class ProfilController : Controller
    {
        private readonly HttpClient _httpClient;

        public ProfilController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        public async Task<IActionResult> Index()
        {
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");
            var token = HttpContext.Session.GetString("JwtToken");

            // Eğer ID yoksa veya Session'da Token yoksa, direkt Login'e yolla
            if (string.IsNullOrEmpty(kullaniciIdStr) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Kullanici");
            }

            // DİKKAT: API'ye giderken cebimizdeki Token'ı kimlik olarak gösteriyoruz!
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // API'ye GET isteği atıyoruz
            var response = await _httpClient.GetAsync($"api/Profil/{kullaniciIdStr}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // API'den gelen veriyi Profil modelimize dönüştürüyoruz
                var profil = JsonSerializer.Deserialize<Profil>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(profil);
            }

            // Eğer API "Unauthorized" (401) dönerse, yani Token'ın süresi dolmuşsa veya sahteyse
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                TempData["Hata"] = "Oturum süreniz doldu, lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Kullanici");
            }

            // Başka bir hata olduysa boş model dön
            return View(new Profil());
        }
    }
}