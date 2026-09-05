# Kütüphane Yönetim Sistemi (RESTful API & ASP.NET Core MVC)

## Açıklama

Bu proje, kütüphane işlemlerini güvenli ve modern bir arayüzle yönetmek için geliştirilmiş kapsamlı bir web uygulamasıdır. JWT tabanlı kimlik doğrulama, rol tabanlı yetkilendirme, RESTful API servisleri, Docker kapsayıcı desteği ve kullanıcı destek/iletişim modüllerini barındırır.

## Teknolojiler

* **Framework:** C# / ASP.NET Core 10.0 (MVC & Web API)
* **Veritabanı:** Entity Framework Core & MSSQL
* **Güvenlik:** JWT (JSON Web Tokens)
* **Konteynerleştirme & CI/CD:** Docker, GitHub Actions
* **İletişim Servisi:** SMTP / E-Posta Gönderme Servisi
* **Arayüz:** HTML5, CSS3, JavaScript (Modern UI & Responsive Tasarım)

## Özellikler

* **Kimlik Doğrulama ve Güvenlik:** JWT tabanlı güvenli kayıt ve giriş (Login/Register) sistemi.
* **Rol Tabanlı Yetkilendirme:** Admin ve Standart Kullanıcı rolleri için ayrıştırılmış yetki yönetimi.
* **Kitap ve Üye Yönetimi:** Kitap ekleme, listeleme, güncelleme ve üye işlemleri.
* **Ödünç / İade Süreçleri:** Kitapların ödünç alınması ve iade edilmesi işlemleri.
* **Docker:** Multi-stage build mimarisine sahip `Dockerfile` entegrasyonu ile uygulamanın tüm platformlarda (Linux/Windows) yalıtılmış ve yüksek performanslı çalışabilmesi.
* **Otomatik CI/CD Akışı:** GitHub Actions ile her `push` ve `pull request` işleminde projenin otomatik derlenmesi (build) ve test edilmesi.
* **Destek & İletişim Modülü:** 
  * Kullanıcıların sistemsel sorunları, soru veya önerilerini e-posta olarak iletebilmesi.
  * Form doğrulama mekanizmaları, yönlendirme zamanlayıcıları ve kullanıcı dostu bildirimler.

## Kurulum ve Çalıştırma

### 1. Yerel Geliştirme Ortamı (IIS Express / Kestrel)
1. `appsettings.json` içerisindeki **ConnectionStrings** (MSSQL veritabanı bağlantısı) ve **EmailSettings** (SMTP e-posta gönderim bilgileri) ayarlarınızı güncelleyin.
2. Visual Studio üst menüsünden çalıştırma profili olarak **IIS Express** veya **KutuphaneYonetim** seçeneğini belirleyin.
3. `F5` tuşuna basarak projeyi başlatın.

### 2. Docker Kapsayıcısı ile Çalıştırma
1. Bilgisayarınızda **Docker Desktop** uygulamasının açık olduğundan emin olun.
2. `appsettings.json` içindeki veritabanı bağlantı adresini `host.docker.internal` (veya SQL Server IP adresi) olarak ayarlayın.
3. Visual Studio üst menüsünden **Docker** profilini seçin ve `F5` ile projeyi konteyner üzerinde başlatın.
   
Alternatif olarak terminal üzerinden şu komut ile imaj oluşturup çalıştırabilirsiniz:
```bash
docker build -t kutuphane-yonetim .
docker run -p 8080:8080 -p 8081:8081 kutuphane-yonetim
