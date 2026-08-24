using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KutuphaneYonetim.DTOs;
using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KutuphaneYonetim.Controllers
{
    public class KategoriController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public KategoriController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        // --- YARDIMCI METODLAR (Hatanın Çözümü) ---

        // 1. Session'daki JWT Token'ı HttpClient istek başlığına ekler
        private bool SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        // 2. Güvenli JSON Çözümleme Metodu
        private bool TryDeserialize<T>(string json, out T? result)
        {
            result = default;
            if (string.IsNullOrWhiteSpace(json)) return false;
            try
            {
                result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                return result != null;
            }
            catch
            {
                return false;
            }
        }

        // --- ACTION METODLARI ---

        // GET: /Kategori/Index
        public async Task<IActionResult> Index()
        {
            List<Kategori> kategoriler = new List<Kategori>();
            var response = await _httpClient.GetAsync("api/Kategori");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                if (TryDeserialize<List<Kategori>>(jsonString, out var list) && list != null)
                {
                    kategoriler = list;
                }
            }

            return View(kategoriler);
        }

        // GET: /Kategori/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: /Kategori/AraAjax
        [HttpGet]
        public async Task<IActionResult> AraAjax(string q)
        {
            List<Kategori> kategoriler = new List<Kategori>();
            var response = await _httpClient.GetAsync($"api/Kategori/Ara?q={q}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                if (TryDeserialize<List<Kategori>>(jsonString, out var list) && list != null)
                {
                    kategoriler = list;
                }
            }

            return PartialView("_KategoriTable", kategoriler);
        }

        // POST: /Kategori/KategoriEkleAjax
        [HttpPost]
        public async Task<IActionResult> KategoriEkleAjax(string KategoriAd)
        {
            if (string.IsNullOrWhiteSpace(KategoriAd))
            {
                return Json(new { success = false, message = "Kategori adı boş olamaz." });
            }

            // Güvenlik için token başlığını ekliyoruz
            SetAuthorizationHeader();

            var dto = new KategoriDto { KategoriAd = KategoriAd };
            var content = new StringContent(JsonSerializer.Serialize(dto, _jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Kategori", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (TryDeserialize<JsonElement>(responseString, out var apiResult))
            {
                bool success = response.IsSuccessStatusCode &&
                               apiResult.TryGetProperty("success", out var s) &&
                               s.ValueKind == JsonValueKind.True;

                string message = apiResult.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String
                    ? m.GetString()!
                    : (success ? "Kategori başarıyla eklendi." : "İşlem başarısız.");

                return Json(new { success, message });
            }

            return Json(new { success = false, message = "Sunucudan geçersiz yanıt alındı." });
        }
    }
}