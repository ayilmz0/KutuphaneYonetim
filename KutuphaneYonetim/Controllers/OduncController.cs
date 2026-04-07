using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IActionResult> Index()
        {
            var uyeIdStr = HttpContext.Session.GetString("UyeId");
            if (!int.TryParse(uyeIdStr, out int uyeId))
            {
                return View(new List<Odunc>());
            }

            List<Odunc> oduncler = new List<Odunc>();

            // API'den sadece bu üyeye ait ödünçleri çekiyoruz
            var response = await _httpClient.GetAsync($"api/Odunc/Uye/{uyeId}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                oduncler = JsonSerializer.Deserialize<List<Odunc>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (!oduncler.Any())
                TempData["Uyari"] = "Henüz ödünç alınmış kitap bulunmamaktadır.";

            return View(oduncler);
        }

        [HttpGet]
        public async Task<IActionResult> OduncAl(int KitapId)
        {
            var uyeIdStr = HttpContext.Session.GetString("UyeId");
            var kullaniciIdStr = HttpContext.Session.GetString("KullaniciId");

            if (!int.TryParse(uyeIdStr, out int uyeId) || !int.TryParse(kullaniciIdStr, out int kullaniciId))
            {
                TempData["Hata"] = "Kitap ödünç almak için giriş yapmalısınız.";
                return RedirectToAction("Index", "Kitap");
            }

            // API'ye göndereceğimiz paketi hazırlıyoruz
            var istekData = new { KitapId = KitapId, UyeId = uyeId, KullaniciId = kullaniciId };
            var content = new StringContent(JsonSerializer.Serialize(istekData), Encoding.UTF8, "application/json");

            // API'ye POST isteği atıyoruz
            var response = await _httpClient.PostAsync("api/Odunc/OduncAl", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<JsonElement>(responseString);

            if (response.IsSuccessStatusCode)
            {
                TempData["Basarili"] = apiResult.GetProperty("message").GetString();
            }
            else
            {
                TempData["Hata"] = apiResult.GetProperty("message").GetString();
            }

            return RedirectToAction("Index", "Kitap");
        }

        [HttpPost]
        public async Task<IActionResult> TeslimEtAjax(int id)
        {
            // İsteği doğrudan API'nin teslim etme ucuna POSTluyoruz
            var response = await _httpClient.PostAsync($"api/Odunc/TeslimEt/{id}", null);
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonSerializer.Deserialize<JsonElement>(responseString);

            bool success = apiResult.GetProperty("success").GetBoolean();
            string message = apiResult.GetProperty("message").GetString();

            return Json(new { success = success, message = message });
        }
    }
}