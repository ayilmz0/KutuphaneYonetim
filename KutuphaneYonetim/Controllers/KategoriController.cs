using KutuphaneYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KutuphaneYonetim.Controllers
{
    public class KategoriController : Controller
    {
        private readonly HttpClient _httpClient;

        public KategoriController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        // GET: /Kategori/Index
        public async Task<IActionResult> Index()
        {
            List<Kategori> kategoriler = new List<Kategori>();
            var response = await _httpClient.GetAsync("api/Kategori");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                kategoriler = JsonSerializer.Deserialize<List<Kategori>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
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
                kategoriler = JsonSerializer.Deserialize<List<Kategori>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return PartialView("_KategoriTable", kategoriler);
        }
    }
}