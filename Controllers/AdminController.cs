using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using DernekSitesi.Models;
using DernekSitesi.Services; // 🚨 ŞİFRELEME MAKİNESİNİ ÇAĞIRDIK!

namespace DernekSitesi.Controllers
{
    public class AdminController : Controller
    {
        private readonly UygulamaDbContext _context;

        public AdminController(UygulamaDbContext context)
        {
            _context = context;
        }

        // 🚨 KAPI GÜVENLİĞİ: Sadece "Admin" rolüne sahip olanlar bu sayfaya girebilir!
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            ViewBag.ToplamUye = await _context.Uyeler.CountAsync();
            ViewBag.AktifUye = await _context.Uyeler.CountAsync(u => u.AktifMi == true);
            ViewBag.ToplamHaber = await _context.Haber.CountAsync();
            ViewBag.OkunmamisMesaj = await _context.Mesajlar.CountAsync(m => m.OkunduMu == false);

            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // 🚨 1. ADIM: Ziyaretçinin forma yazdığı düz şifreyi kriptoluyoruz! 🚨
            string kriptoluSifre = Sifreleme.HashSifre(password);

            // 🚨 2. ADIM: Veritabanında E-postası ve KRİPTOLANMIŞ ŞİFRESİ eşleşen kişiyi arıyoruz 🚨
            var uye = await _context.Uyeler.FirstOrDefaultAsync(u => u.Email == username && u.Sifre == kriptoluSifre);

            // Eğer veritabanında kriptolu şifre eşleştiyse VEYA kurucu admin (Acil durum kapısı) giriyorsa
            if (uye != null || (username == "admin" && password == "12345"))
            {
                // 🎫 VIP BİLETİ KESİMİ: Giren kişi hayalet adminse yetkisi "Admin" olur, veritabanından biriyse veritabanındaki "Yetki"si okunur.
                string girisYapaninYetkisi = (username == "admin") ? "Admin" : uye.Yetki;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, girisYapaninYetkisi) // 🎫 BİLETE YETKİ DAMGASI VURULDU!
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index");
            }

            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }
        // KAPI GÜVENLİĞİNE TAKILANLARIN DÜŞECEĞİ SAYFA
        public IActionResult ErisimEngellendi()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // 🚨 KAPI GÜVENLİĞİ: Sadece "Admin" rolüne sahip olanlar mesajları okuyabilir!
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GelenKutusu()
        {
            var mesajlar = await _context.Mesajlar
                .OrderByDescending(m => m.GonderimTarihi)
                .ToListAsync();

            return View(mesajlar);
        }

        public async Task<IActionResult> MesajOkundu(int id)
        {
            var mesaj = await _context.Mesajlar.FindAsync(id);

            if (mesaj != null)
            {
                mesaj.OkunduMu = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("GelenKutusu");
        }
    }
}