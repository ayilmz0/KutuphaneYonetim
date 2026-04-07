using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace KutuphaneYonetim.Controllers
{
    public class OduncController : Controller
    {
        private readonly HttpClient _httpClient;

        public OduncController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        // Token'ı HttpClient'a eklemek için yardımcı bir metod (Sürekli aynı kodu yazmamak için)
        private bool SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        public async Task<IActionResult> Index()
        {
            var uyeIdStr = HttpContext.Session.GetString("UyeId");
            if (!SetAuthorizationHeader() || string.IsNullOrEmpty(uyeIdStr))
            {
                return RedirectToAction("Login", "Kullanici");
            }

            List<Odunc> oduncler = new List<Odunc>();

            // Artık API'ye giderken başlığımızda Token var!
            var response = await _httpClient.GetAsync($"api/Odunc/Uye/{uyeIdStr}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                oduncler = JsonSerializer.Deserialize<List<Odunc>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Token süresi dolmuşsa
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Kullanici");
            }

            if (!oduncler.Any())
                TempData["Uyari"] = "Henüz ödünç alınmış kitap bulunmamaktadır.";

            return View(oduncler);
        }

        [HttpGet]
        public async Task<IActionResult> OduncAl(int KitapId)
        {
            if (!SetAuthorizationHeader())
            {
                TempData["Hata"] = "Kitap ödünç almak için giriş yapmalısınız.";
                return RedirectToAction("Login", "Kullanici");
            }

            // DİKKAT: Artık API'ye UyeId veya KullaniciId GÖNDERMİYORUZ!
            // Sadece hangi kitabı istediğimizi söylüyoruz. API bizim kim olduğumuzu Token'dan bilecek.
            var istekData = new { KitapId = KitapId };
            var content = new StringContent(JsonSerializer.Serialize(istekData), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Odunc/OduncAl", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<JsonElement>(responseString);

            if (response.IsSuccessStatusCode)
            {
                TempData["Basarili"] = apiResult.GetProperty("message").GetString();
            }
            else
            {
                TempData["Hata"] = apiResult.TryGetProperty("message", out var msg) ? msg.GetString() : "Bir hata oluştu.";
            }

            return RedirectToAction("Index", "Kitap");
        }

        [HttpPost]
        public async Task<IActionResult> TeslimEtAjax(int id)
        {
            if (!SetAuthorizationHeader())
            {
                return Json(new { success = false, message = "Oturum süreniz dolmuş." });
            }

            var response = await _httpClient.PostAsync($"api/Odunc/TeslimEt/{id}", null);
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<JsonElement>(responseString);

            bool success = apiResult.TryGetProperty("success", out var succ) && succ.GetBoolean();
            string message = apiResult.TryGetProperty("message", out var msg) ? msg.GetString() : "İşlem sonucu okunamadı.";

            return Json(new { success = success, message = message });
        }
    }
}