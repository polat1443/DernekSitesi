using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DernekSitesi.Models;
using Microsoft.AspNetCore.Http; // Resim yüklemek (IFormFile) için gerekli
using System.IO; // Klasör yollarını (Path) bulmak için gerekli
using DernekSitesi.Services; // 👈 POSTACIMIZ İÇİN EKLENDİ

namespace DernekSitesi.Controllers
{
    public class HabersController : Controller
    {
        private readonly UygulamaDbContext _context;
        private readonly EmailService _emailService; // 👈 POSTACI TANIMLANDI

        // YAPIICI METOT GÜNCELLENDİ (POSTACI İÇERİ ALINDI)
        public HabersController(UygulamaDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: Habers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Haber.ToListAsync());
        }

        // GET: Habers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var haber = await _context.Haber
                .FirstOrDefaultAsync(m => m.Id == id);
            if (haber == null)
            {
                return NotFound();
            }

            return View(haber);
        }

        // GET: Habers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Habers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // DİKKAT: Metoda resimDosyasi parametresi ve Bind içine ResimYolu eklendi
        public async Task<IActionResult> Create([Bind("Id,Baslik,Icerik,YayinTarihi,IsPriority,ResimYolu")] Haber haber, IFormFile? resimDosyasi)
        {
            if (ModelState.IsValid)
            {
                // RESİM YÜKLEME KODU BAŞLANGICI
                if (resimDosyasi != null && resimDosyasi.Length > 0)
                {
                    string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                    string kayitYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/haberler", benzersizIsim);

                    // Eğer img/haberler klasörü yoksa otomatik oluştursun (Hata vermesin diye)
                    var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/haberler");
                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    using (var stream = new FileStream(kayitYolu, FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(stream);
                    }

                    haber.ResimYolu = "/img/haberler/" + benzersizIsim;
                }
                // RESİM YÜKLEME KODU BİTİŞİ

                _context.Add(haber);
                await _context.SaveChangesAsync(); // 💥 HABER KAYDEDİLDİ!

                // -------------------------------------------------------------------------
                // 🚀 OLAY TABANLI (EVENT-DRIVEN) OTOMATİK MAİL GÖNDERİMİ BAŞLIYOR 🚀
                // -------------------------------------------------------------------------

                // Veritabanındaki e-posta adresi dolu olan tüm üyelerin maillerini topluyoruz
                var uyeMailleri = await _context.Uyeler
                    .Where(u => !string.IsNullOrEmpty(u.Email))
                    .Select(u => u.Email)
                    .ToListAsync();

                if (uyeMailleri.Any()) // Eğer sistemde maili olan üye varsa
                {
                    // Şık bir HTML mail hazırlıyoruz
                    string konu = "📰 Yeni Haber: " + haber.Baslik;
                    string icerik = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #00b894;'>Sayın Üyemiz,</h2>
                            <p>Sitemizde yeni bir haber yayınlandı:</p>
                            <h3 style='color: #2d3436;'>{haber.Baslik}</h3>
                            <p style='color: #636e72;'>{haber.Icerik}</p>
                            <br>
                            <a href='http://polatbilir-001-site1.mtempurl.com/Habers/Details/{haber.Id}' 
                               style='background-color: #00b894; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                               Haberi Oku
                            </a>
                            <hr style='margin-top: 30px;'>
                            <p style='font-size: 12px; color: #b2bec3;'>Bu e-posta otomasyon sistemi tarafından gönderilmiştir.</p>
                        </div>";

                    // Postacımıza 'Git ve bu mailleri dağıt!' emrini veriyoruz
                    await _emailService.TopluMailGonderAsync(uyeMailleri, konu, icerik);
                }
                // -------------------------------------------------------------------------

                return RedirectToAction(nameof(Index));
            }
            return View(haber);
        }

        // GET: Habers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var haber = await _context.Haber.FindAsync(id);
            if (haber == null)
            {
                return NotFound();
            }
            return View(haber);
        }

        // POST: Habers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // DİKKAT: Metoda resimDosyasi parametresi ve Bind içine ResimYolu eklendi
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Icerik,YayinTarihi,IsPriority,ResimYolu")] Haber haber, IFormFile? resimDosyasi)
        {
            if (id != haber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // YENİ RESİM GÜNCELLEME KODU BAŞLANGICI
                    if (resimDosyasi != null && resimDosyasi.Length > 0)
                    {
                        string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                        string kayitYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/haberler", benzersizIsim);

                        // Eğer img/haberler klasörü yoksa otomatik oluştursun
                        var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/haberler");
                        if (!Directory.Exists(klasorYolu))
                        {
                            Directory.CreateDirectory(klasorYolu);
                        }

                        using (var stream = new FileStream(kayitYolu, FileMode.Create))
                        {
                            await resimDosyasi.CopyToAsync(stream);
                        }

                        // Yeni resmin yolunu atıyoruz (Eskisini eziyor)
                        haber.ResimYolu = "/img/haberler/" + benzersizIsim;
                    }
                    // YENİ RESİM GÜNCELLEME KODU BİTİŞİ
                    // (Eğer resim seçilmemişse View'den gelen eski ResimYolu hidden input sayesinde korunur)

                    _context.Update(haber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HaberExists(haber.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(haber);
        }

        // GET: Habers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var haber = await _context.Haber
                .FirstOrDefaultAsync(m => m.Id == id);
            if (haber == null)
            {
                return NotFound();
            }

            return View(haber);
        }

        // POST: Habers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var haber = await _context.Haber.FindAsync(id);
            if (haber != null)
            {
                _context.Haber.Remove(haber);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HaberExists(int id)
        {
            return _context.Haber.Any(e => e.Id == id);
        }
    }
}