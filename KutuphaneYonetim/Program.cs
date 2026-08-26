using KutuphaneYonetim.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KutuphaneYonetim.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddHttpContextAccessor();

// appsettings.json içindeki EmailSettings bölümünü okuyup servislere ekler
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Email servisinizi DI container'a kaydedin (Scoped veya Transient yapabilirsiniz)
builder.Services.AddScoped<IEmailServis, EmailServis>();

// DbContext
builder.Services.AddDbContext<KutuphaneYonetimContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ Session servisleri
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// JWT Ayarlarını appsettings.json'dan okuma
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];

// Kimlik Doğrulama (Authentication) servisini JWT olarak ayarlama
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Kartı basan doğru kişi mi?
        ValidateAudience = true, // Kart doğru yere mi verilmiş?
        ValidateLifetime = true, // Kartın süresi dolmuş mu?
        ValidateIssuerSigningKey = true, // İmza doğru mu?
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// ✅ Session middleware
app.UseSession();

app.UseAuthorization();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=AnaSayfa}/{id?}");

app.Run();
