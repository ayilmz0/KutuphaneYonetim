using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace KutuphaneYonetim.Controllers
{
    public class KitapController : Controller
    {
        private readonly HttpClient _httpClient;

        public KitapController(IHttpClientFactory httpClientFactory)
        {
            // API'ye istek atabilmek için HttpClient oluşturuyoruz
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        // GET: /Kitap/Index
        public async Task<IActionResult> Index()
        {
            List<Kitap> kitaplar = new List<Kitap>();
            var response = await _httpClient.GetAsync("api/Kitap");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                kitaplar = JsonSerializer.Deserialize<List<Kitap>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return View(kitaplar);
        }

        // GET: /Kitap/Create
        public async Task<IActionResult> Create()
        {
            List<Kategori> kategoriler = new List<Kategori>();
            var response = await _httpClient.GetAsync("api/Kitap/Kategoriler");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                kategoriler = JsonSerializer.Deserialize<List<Kategori>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            ViewBag.Kategori = new SelectList(kategoriler, "KategoriId", "KategoriAd");
            return View();
        }

        // GET: /Kitap/Update/5
        public async Task<IActionResult> Update(int id)
        {
            Kitap kitap = null;
            var response = await _httpClient.GetAsync($"api/Kitap/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                kitap = JsonSerializer.Deserialize<Kitap>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            if (kitap == null) return NotFound();

            // Kategorileri de çekelim
            var katResponse = await _httpClient.GetAsync("api/Kitap/Kategoriler");
            if (katResponse.IsSuccessStatusCode)
            {
                var katJson = await katResponse.Content.ReadAsStringAsync();
                var kategoriler = JsonSerializer.Deserialize<List<Kategori>>(katJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Kategori = new SelectList(kategoriler, "KategoriId", "KategoriAd", kitap.KategoriId);
            }

            return View(kitap);
        }

        [HttpGet]
        public async Task<IActionResult> AraAjax(string q)
        {
            List<Kitap> kitaplar = new List<Kitap>();

            // API'nin arama ucuna istek atıyoruz
            var response = await _httpClient.GetAsync($"api/Kitap/Ara?q={q}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                kitaplar = JsonSerializer.Deserialize<List<Kitap>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            // JSON verisini aldık, eski sistemdeki gibi PartialView olarak HTML'e çevirip JQuery'e veriyoruz
            return PartialView("_KitapTable", kitaplar);
        }
    }
}