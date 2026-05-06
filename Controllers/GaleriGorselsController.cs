using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DernekSitesi.Models;
using Microsoft.AspNetCore.Http; // Resim yükleme işlemleri için eklendi
using System.IO; // Klasör ve yol işlemleri için eklendi

namespace DernekSitesi.Controllers
{
    public class GaleriGorselsController : Controller
    {
        private readonly UygulamaDbContext _context;

        public GaleriGorselsController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: GaleriGorsels
        public async Task<IActionResult> Index()
        {
            return View(await _context.GaleriGorseller.ToListAsync());
        }

        // GET: GaleriGorsels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galeriGorsel = await _context.GaleriGorseller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (galeriGorsel == null)
            {
                return NotFound();
            }

            return View(galeriGorsel);
        }

        // GET: GaleriGorsels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GaleriGorsels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Baslik,ResimYolu,CokluFotograflar,EklenmeTarihi")] GaleriGorsel galeriGorsel, IFormFile? resimDosyasi, List<IFormFile>? cokluDosyalar)
        {
            if (ModelState.IsValid)
            {
                // 1. KAPAK FOTOĞRAFINI YÜKLEME KODU
                if (resimDosyasi != null && resimDosyasi.Length > 0)
                {
                    string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                    string kayitYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/galeri", benzersizIsim);

                    var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/galeri");
                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    using (var stream = new FileStream(kayitYolu, FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(stream);
                    }

                    galeriGorsel.ResimYolu = "/img/galeri/" + benzersizIsim;
                }

                // 2. ÇOKLU (ETKİNLİK İÇİ) FOTOĞRAFLARI YÜKLEME KODU
                if (cokluDosyalar != null && cokluDosyalar.Count > 0)
                {
                    List<string> yuklenenYollar = new List<string>();
                    foreach (var dosya in cokluDosyalar)
                    {
                        if (dosya.Length > 0)
                        {
                            string fotoIsmi = Guid.NewGuid().ToString() + Path.GetExtension(dosya.FileName);
                            string fotoYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/galeri", fotoIsmi);

                            using (var stream = new FileStream(fotoYolu, FileMode.Create))
                            {
                                await dosya.CopyToAsync(stream);
                            }
                            // Yüklenen her bir fotoğrafın yolunu listeye ekle
                            yuklenenYollar.Add("/img/galeri/" + fotoIsmi);
                        }
                    }
                    // Tüm yolları arasına virgül koyarak veritabanına tek bir satırda kaydet
                    galeriGorsel.CokluFotograflar = string.Join(",", yuklenenYollar);
                }

                _context.Add(galeriGorsel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(galeriGorsel);
        }

        // GET: GaleriGorsels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galeriGorsel = await _context.GaleriGorseller.FindAsync(id);
            if (galeriGorsel == null)
            {
                return NotFound();
            }
            return View(galeriGorsel);
        }

        // POST: GaleriGorsels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // DİKKAT: silinecekFotolar parametresi eklendi
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,ResimYolu,CokluFotograflar,EklenmeTarihi")] GaleriGorsel galeriGorsel, IFormFile? resimDosyasi, List<IFormFile>? cokluDosyalar, List<string>? silinecekFotolar)
        {
            if (id != galeriGorsel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. YENİ KAPAK RESMİ GÜNCELLEME KODU
                    if (resimDosyasi != null && resimDosyasi.Length > 0)
                    {
                        string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                        string kayitYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/galeri", benzersizIsim);

                        var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/galeri");
                        if (!Directory.Exists(klasorYolu))
                        {
                            Directory.CreateDirectory(klasorYolu);
                        }

                        using (var stream = new FileStream(kayitYolu, FileMode.Create))
                        {
                            await resimDosyasi.CopyToAsync(stream);
                        }

                        galeriGorsel.ResimYolu = "/img/galeri/" + benzersizIsim;
                    }

                    // 2. ÇOKLU FOTOĞRAFLARI YÖNETME (SİLME VE EKLEME)
                    List<string> kalanYollar = new List<string>();

                    // Önce eski fotoğrafların yollarını listeye alıyoruz
                    if (!string.IsNullOrEmpty(galeriGorsel.CokluFotograflar))
                    {
                        kalanYollar = galeriGorsel.CokluFotograflar.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    }

                    // EĞER SİLİNMESİ İSTENEN FOTOĞRAFLAR SEÇİLMİŞSE:
                    if (silinecekFotolar != null && silinecekFotolar.Count > 0)
                    {
                        foreach (var silinecek in silinecekFotolar)
                        {
                            kalanYollar.Remove(silinecek); // Resmin yolunu veritabanı listesinden çıkar

                            // Sunucuda (bilgisayarda) boşuna yer kaplamaması için fiziksel dosyayı da sil
                            string fizikselYol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", silinecek.TrimStart('/'));
                            if (System.IO.File.Exists(fizikselYol))
                            {
                                System.IO.File.Delete(fizikselYol);
                            }
                        }
                    }

                    // EĞER ALBÜME YENİ FOTOĞRAFLAR EKLENMİŞSE (Eskilerin yanına ekle)
                    if (cokluDosyalar != null && cokluDosyalar.Count > 0)
                    {
                        foreach (var dosya in cokluDosyalar)
                        {
                            if (dosya.Length > 0)
                            {
                                string fotoIsmi = Guid.NewGuid().ToString() + Path.GetExtension(dosya.FileName);
                                string fotoYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/galeri", fotoIsmi);

                                using (var stream = new FileStream(fotoYolu, FileMode.Create))
                                {
                                    await dosya.CopyToAsync(stream);
                                }

                                kalanYollar.Add("/img/galeri/" + fotoIsmi); // Yeni resmi de listeye ekle
                            }
                        }
                    }

                    // Kalan/Eklenen tüm resim yollarını tekrar virgülle birleştirip sisteme kaydet
                    galeriGorsel.CokluFotograflar = string.Join(",", kalanYollar);

                    _context.Update(galeriGorsel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GaleriGorselExists(galeriGorsel.Id))
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
            return View(galeriGorsel);
        }

        // GET: GaleriGorsels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var galeriGorsel = await _context.GaleriGorseller
                .FirstOrDefaultAsync(m => m.Id == id);
            if (galeriGorsel == null)
            {
                return NotFound();
            }

            return View(galeriGorsel);
        }

        // POST: GaleriGorsels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var galeriGorsel = await _context.GaleriGorseller.FindAsync(id);
            if (galeriGorsel != null)
            {
                _context.GaleriGorseller.Remove(galeriGorsel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GaleriGorselExists(int id)
        {
            return _context.GaleriGorseller.Any(e => e.Id == id);
        }
    }
}