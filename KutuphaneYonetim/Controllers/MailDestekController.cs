using Microsoft.AspNetCore.Mvc;
using KutuphaneYonetim.Models;

namespace KutuphaneYonetim.Controllers
{
    public class MailDestekController : Controller
    {
        private readonly IEmailServis _emailServis;

        // Dependency Injection ile Email servisini çağırıyoruz
        public MailDestekController(IEmailServis emailServis)
        {
            _emailServis = emailServis;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MailForm()
        {
            return View("~/Views/Mail/MailForm.cshtml");
        }

        [HttpPost]
        public IActionResult MesajGonder(DestekFormuDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Hata"] = "Lütfen formdaki tüm zorunlu alanları doldurun.";
                return View("~/Views/Mail/MailForm.cshtml", dto);
            }

            bool sonuc = _emailServis.DestekMaileGonder(dto);

            if (sonuc)
            {
                TempData["Basarili"] = "Destek mesajınız başarıyla gönderildi. En kısa sürede dönüş yapacağız.";

                ModelState.Clear();

                return View("~/Views/Mail/MailForm.cshtml", new DestekFormuDto());
            }
            else
            {
                TempData["Hata"] = "Mesaj gönderilirken bir hata oluştu.";
                return View("~/Views/Mail/MailForm.cshtml", dto);
            }
        }
    }
}