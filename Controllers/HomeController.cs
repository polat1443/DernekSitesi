using Microsoft.AspNetCore.Mvc;
using DernekSitesi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DernekSitesi.Controllers
{
    public class HomeController : Controller
    {
        private readonly UygulamaDbContext _context;

        public HomeController(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Yaklaþan ilk toplantýyý alýyoruz
            var enYakinToplanti = await _context.Toplantilar
                .Where(t => t.GelecekToplantiMi == true)
                .OrderBy(t => t.Tarih)
                .Take(1)
                .ToListAsync();

            // 2. Aktif üye sayýsýný alýyoruz
            int aktifUyeSayisi = await _context.Uyeler.CountAsync(u => u.AktifMi == true);
            ViewBag.UyeSayisi = aktifUyeSayisi;

            // 3. Son 3 Haberi alýp vitrine gönderiyoruz
            var sonHaberler = await _context.Haber
                .OrderByDescending(h => h.YayinTarihi)
                .Take(3)
                .ToListAsync();
            ViewBag.SonHaberler = sonHaberler;

            // 4. Slayt (Slider) için galerideki son 5 kapak resmini alýyoruz
            var sliderGorseller = await _context.GaleriGorseller
                .OrderByDescending(g => g.EklenmeTarihi)
                .Take(5)
                .ToListAsync();
            ViewBag.SliderGorseller = sliderGorseller;

            // 5. YENÝ: Yönetim Kurulunu vitrine gönderiyoruz (Sýralama numarasýna göre dizili)
            var yonetimKurulu = await _context.YonetimUyeleri
                .OrderBy(y => y.Siralama)
                .ToListAsync();
            ViewBag.YonetimKadrosu = yonetimKurulu;

            return View(enYakinToplanti);
        }

        [HttpPost]
        public async Task<IActionResult> MesajGonder(Mesaj yeniMesaj)
        {
            if (ModelState.IsValid)
            {
                yeniMesaj.GonderimTarihi = DateTime.Now;
                yeniMesaj.OkunduMu = false;

                _context.Add(yeniMesaj);
                await _context.SaveChangesAsync();

                try
                {
                    string seninMailin = "polatb1443@gmail.com";
                    string uygulamaSifresi = "oxph vaba keqj wycr";

                    var smtpClient = new SmtpClient("smtp.gmail.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential(seninMailin, uygulamaSifresi),
                        EnableSsl = true,
                    };

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(seninMailin, "Dernek Sitesi Ýletiþim Formu"),
                        Subject = $"Siteden Yeni Mesaj: {yeniMesaj.Konu}",
                        Body = $@"
                            <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                                <h2 style='color: #2b5876;'>Siteden Yeni Bir Mesajýnýz Var! ??</h2>
                                <hr/>
                                <p><b>Gönderen:</b> {yeniMesaj.AdSoyad}</p>
                                <p><b>E-Posta:</b> {yeniMesaj.Email}</p>
                                <p><b>Konu:</b> {yeniMesaj.Konu}</p>
                                <p><b>Mesaj Ýçeriði:</b></p>
                                <div style='background-color: #f9f9f9; padding: 15px; border-radius: 5px; border-left: 4px solid #f39c12;'>
                                    {yeniMesaj.Icerik}
                                </div>
                                <br/>
                                <small style='color: #888;'>Bu bildirim Dernek Yönetim Sistemi tarafýndan otomatik olarak gönderilmiþtir.</small>
                            </div>",
                        IsBodyHtml = true,
                    };

                    mailMessage.To.Add(seninMailin);
                    await smtpClient.SendMailAsync(mailMessage);
                }
                catch (Exception)
                {
                    // Hata yutulur
                }

                TempData["MesajDurum"] = "Basarili";
                return RedirectToAction("Index");
            }

            TempData["MesajDurum"] = "Hata";
            return RedirectToAction("Index");
        }

        public IActionResult Vizyon()
        {
            return View();
        }

        public IActionResult Misyon()
        {
            return View();
        }

        public IActionResult Galeri()
        {
            var resimler = _context.GaleriGorseller.OrderByDescending(g => g.EklenmeTarihi).ToList();
            return View(resimler);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}