using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class OduncController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public OduncController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:44379/");
        }

        private bool SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return true;
        }

        private static Odunc MapDtoToOdunc(OduncDetailDto dto)
        {
            if (dto == null) return null!;
            return new Odunc
            {
                OduncId = dto.OduncId,
                KullaniciId = dto.KullaniciId,
                KitapId = dto.KitapId,
                UyeId = dto.UyeId,
                AlisTarihi = dto.AlisTarihi,
                IadeTarihi = dto.IadeTarihi,
                Ceza = dto.Ceza,
                Durum = dto.Durum,
                Kitap = dto.Kitap != null ? new Kitap
                {
                    KitapId = dto.Kitap.KitapId,
                    KategoriId = dto.Kitap.KategoriId,
                    KitapAd = dto.Kitap.KitapAd,
                    Yazar = dto.Kitap.Yazar,
                    YayinEvi = dto.Kitap.YayinEvi,
                    SayfaSayisi = dto.Kitap.SayfaSayisi,
                    ISBN = dto.Kitap.ISBN,
                    Stok = dto.Kitap.Stok,
                    Durum = dto.Kitap.Durum
                } : null,
                Uye = dto.Uye != null ? new Uye
                {
                    UyeId = dto.Uye.UyeId,
                    KullaniciId = dto.Uye.KullaniciId,
                    Ad = dto.Uye.Ad,
                    Soyad = dto.Uye.Soyad,
                    KayitTarihi = dto.Uye.KayitTarihi,
                    Durum = dto.Uye.Durum
                } : null
            };
        }

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

        public async Task<IActionResult> Index()
        {
            var uyeIdStr = HttpContext.Session.GetString("UyeId");
            if (!SetAuthorizationHeader() || string.IsNullOrEmpty(uyeIdStr))
            {
                return RedirectToAction("Login", "Kullanici");
            }

            List<Odunc> oduncler = new List<Odunc>();
            var requestUrl = $"api/Odunc/Uye/{uyeIdStr}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Kullanici");
            }

            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode && TryDeserialize<List<OduncDetailDto>>(responseString, out var dtoList) && dtoList != null)
            {
                oduncler = dtoList.Select(MapDtoToOdunc).ToList();
            }

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

            var istekData = new { KitapId = KitapId };
            var content = new StringContent(JsonSerializer.Serialize(istekData, _jsonOptions), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Odunc/OduncAl", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                TempData["Hata"] = "Oturumunuz sonlanmış. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Kullanici");
            }

            if (TryDeserialize<JsonElement>(responseString, out var apiResult))
            {
                if (response.IsSuccessStatusCode)
                {
                    if (apiResult.TryGetProperty("message", out var m) && m.ValueKind == JsonValueKind.String)
                        TempData["Basarili"] = m.GetString();
                    else
                        TempData["Basarili"] = "İşlem başarılı.";
                }
                else
                {
                    string hata = apiResult.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String
                        ? msg.GetString() ?? "Bir hata oluştu."
                        : $"Sunucu hatası: {response.StatusCode}";
                    TempData["Hata"] = hata;
                }
            }
            else
            {
                TempData["Hata"] = response.IsSuccessStatusCode ? "İşlem tamamlandı fakat sunucudan beklenmeyen cevap alındı." : $"İstek başarısız: {response.StatusCode}";
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

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                HttpContext.Session.Clear();
                return Json(new { success = false, message = "Oturumunuz sonlandı." });
            }

            if (TryDeserialize<JsonElement>(responseString, out var apiResult))
            {
                bool success = apiResult.TryGetProperty("success", out var succ) && (succ.ValueKind == JsonValueKind.True || (succ.ValueKind == JsonValueKind.String && succ.GetString()?.ToLower() == "true"));
                string message = apiResult.TryGetProperty("message", out var msg) && msg.ValueKind == JsonValueKind.String ? msg.GetString()! : "İşlem yapıldı.";
                return Json(new { success, message });
            }

            return Json(new { success = false, message = response.IsSuccessStatusCode ? "Sunucudan beklenmeyen cevap geldi." : $"İstek başarısız: {response.StatusCode}" });
        }
    }
}