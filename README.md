# Kütüphane Yönetim Sistemi (RESTful API & ASP.NET Core MVC)

## Açıklama

Bu proje, kütüphane işlemlerini güvenli ve modern bir arayüzle yönetmek için geliştirilmiş kapsamlı bir web uygulamasıdır. JWT tabanlı kimlik doğrulama, rol tabanlı yetkilendirme, RESTful API servisleri ve kullanıcı destek/iletişim modüllerini barındırır.

## Teknolojiler

* C# / ASP.NET Core MVC & Web API
* Entity Framework Core
* MSSQL
* JWT (JSON Web Tokens)
* SMTP / E-Posta Gönderme Servisi
* HTML5, CSS3, JavaScript (Modern UI & Responsive Tasarım)

## Özellikler

* **Kimlik Doğrulama ve Güvenlik:** JWT tabanlı güvenli kayıt ve giriş (Login/Register) sistemi.
* **Rol Tabanlı Yetkilendirme:** Admin ve Standart Kullanıcı rolleri için ayrıştırılmış yetki yönetimi.
* **Kitap ve Üye Yönetimi:** Kitap ekleme, listeleme, güncelleme ve üye işlemleri.
* **Ödünç / İade Süreçleri:** Kitapların ödünç alınması ve iade edilmesi işlemleri.
* **Destek & İletişim Modülü**.
* **RESTful Mimari:** Endpoints tasarımı ve iyi uygulama standartlarına uygunluk.
* **Kapsamlı Hata Yönetimi:** Veri doğrulama ve kullanıcı dostu hata bildirimleri.

## Kurulum ve Çalıştırma

1. Projeyi klonlayın veya bilgisayarınıza indirin.
2. `appsettings.json` dosyası içerisindeki **ConnectionStrings** (MSSQL veritabanı bağlantısı) ve **EmailSettings** (SMTP e-posta gönderim bilgileri) ayarlarınızı kendi bilgilerinize göre güncelleyin.
3. Visual Studio üzerinden projeyi **Build** edin.
4. `F5` tuşuna basarak veya terminal üzerinden `dotnet run` komutu ile projeyi çalıştırın.
