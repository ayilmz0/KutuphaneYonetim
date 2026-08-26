namespace KutuphaneYonetim.Models
{

    public class DestekFormuDto
    {
        public string GonderenAdSoyad { get; set; }
        public string GonderenEmail { get; set; }
        public string Konu { get; set; }
        public string Mesaj { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string KendiEmailim { get; set; }
        public string UygulamaSifresi { get; set; }
    }
}