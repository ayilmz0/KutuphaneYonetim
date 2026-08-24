using KutuphaneYonetim.Models;
using KutuphaneYonetim.DTOs;
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

        // Helper: DTO -> Entity mapping
        private Kitap MapDtoToKitap(KitapDetailDto dto)
        {
            if (dto == null) return null;
            return new Kitap
            {
                KitapId = dto.KitapId,
                KategoriId = dto.KategoriId,
                KitapAd = dto.KitapAd,
                Yazar = dto.Yazar,
                YayinEvi = dto.YayinEvi,
                SayfaSayisi = dto.SayfaSayisi,
                ISBN = dto.ISBN,
                Stok = dto.Stok,
                Durum = dto.Durum,
                Kategori = dto.Kategori != null ? new Kategori
                {
                    KategoriId = dto.Kategori.KategoriId,
                    KategoriAd = dto.Kategori.KategoriAd
                } : null
            };
        }

        // GET: /Kitap/Index
        public async Task<IActionResult> Index()
        {
            List<Kitap> kitaplar = new List<Kitap>();
            var response = await _httpClient.GetAsync("api/Kitap");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var dtoList = JsonSerializer.Deserialize<List<KitapDetailDto>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dtoList != null)
                {
                    kitaplar = dtoList.Select(MapDtoToKitap).ToList();
                }
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
                var kategoriDtos = JsonSerializer.Deserialize<List<KategoriDto>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (kategoriDtos != null)
                {
                    kategoriler = kategoriDtos.Select(d => new Kategori { KategoriId = d.KategoriId, KategoriAd = d.KategoriAd }).ToList();
                }
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
                var dto = JsonSerializer.Deserialize<KitapDetailDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                kitap = MapDtoToKitap(dto);
            }

            if (kitap == null) return NotFound();

            // Kategorileri de çekelim
            var katResponse = await _httpClient.GetAsync("api/Kitap/Kategoriler");
            if (katResponse.IsSuccessStatusCode)
            {
                var katJson = await katResponse.Content.ReadAsStringAsync();
                var kategoriDtos = JsonSerializer.Deserialize<List<KategoriDto>>(katJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                var kategoriler = kategoriDtos?.Select(d => new Kategori { KategoriId = d.KategoriId, KategoriAd = d.KategoriAd }).ToList() ?? new List<Kategori>();
                ViewBag.Kategori = new SelectList(kategoriler, "KategoriId", "KategoriAd", kitap.KategoriId);
            }

            return View(kitap);
        }

        [HttpGet]
        public async Task<IActionResult> AraAjax(string q)
        {
            List<Kitap> kitaplar = new List<Kitap>();

            // API'nin arama ucuna istek atıyoruz
            var response = await _httpClient.GetAsync($"api/Kitap/Ara?q={System.Net.WebUtility.UrlEncode(q ?? string.Empty)}");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var dtoList = JsonSerializer.Deserialize<List<KitapDetailDto>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (dtoList != null)
                {
                    kitaplar = dtoList.Select(MapDtoToKitap).ToList();
                }
            }

            // JSON verisini aldık, eski sistemdeki gibi PartialView olarak HTML'e çevirip JQuery'e veriyoruz
            return PartialView("_KitapTable", kitaplar);
        }
    }
}