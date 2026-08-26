using System;
using System.Net;
using System.Net.Mail;
using KutuphaneYonetim.Models;
using Microsoft.Extensions.Options;

public interface IEmailServis
{
    bool DestekMaileGonder(DestekFormuDto dto);
}

public class EmailServis : IEmailServis
{
    private readonly EmailSettings _emailSettings;

    // appsettings.json verileri IOptions ile buraya otomatik gelir
    public EmailServis(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public bool DestekMaileGonder(DestekFormuDto dto)
    {
        try
        {
            var mail = new MailMessage();
            mail.From = new MailAddress(_emailSettings.KendiEmailim, "Kütüphane Destek");
            mail.To.Add(_emailSettings.KendiEmailim); // Size ulaşması için
            mail.Subject = $"[Kütüphane Destek] - {dto.Konu}";

            // Size gelen maile "Yanıtla" dediğinizde mesaj atan kullanıcının maili otomatik seçilsin diye:
            if (!string.IsNullOrEmpty(dto.GonderenEmail))
            {
                mail.ReplyToList.Add(new MailAddress(dto.GonderenEmail));
            }

            mail.Body = $@"
                <h3>Yeni Destek Mesajı</h3>
                <p><strong>Gönderen:</strong> {dto.GonderenAdSoyad} ({dto.GonderenEmail})</p>
                <p><strong>Konu:</strong> {dto.Konu}</p>
                <hr/>
                <p><strong>Mesaj:</strong></p>
                <p>{dto.Mesaj}</p>";
            mail.IsBodyHtml = true;

            using (var smtp = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort))
            {
                smtp.Credentials = new NetworkCredential(_emailSettings.KendiEmailim, _emailSettings.UygulamaSifresi);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Mail gönderme hatası: " + ex.Message);
            return false;
        }
    }
}