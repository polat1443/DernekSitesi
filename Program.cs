using Microsoft.AspNetCore.Authentication.Cookies; // GÜVENLÝK ÝÇÝN EKLENDÝ
using Microsoft.EntityFrameworkCore;
using DernekSitesi.Models;
using DernekSitesi.Services; // ?? POSTACIMIZIN KLASÖRÜNÜ BEYNE TANITTIK

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ?? POSTACIMIZI RESMEN ÝÞE ALDIK (YENÝ EKLENDÝ) ??
builder.Services.AddScoped<EmailService>();

builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// SÝTEYE KÝMLÝK DOÐRULAMA (ÞÝFRE) SÝSTEMÝNÝ EKLÝYORUZ
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login"; // Giriþ yapmayanlarý bu sayfaya at
        options.AccessDeniedPath = "/Admin/ErisimEngellendi"; // ?? YETKÝSÝ OLMAYANLARI YASAK BÖLGEYE YOLLA (YENÝ EKLENDÝ)
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // ÖNCE KÝMLÝK SOR
app.UseAuthorization();  // SONRA YETKÝ VER

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();