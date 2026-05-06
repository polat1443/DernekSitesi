using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DernekSitesi.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using ClosedXML.Excel;
using DernekSitesi.Services; // ŞİFRELEME İÇİN GEREKLİ (Zaten eklemiştin)

namespace DernekSitesi.Controllers
{
    public class UyesController : Controller
    {
        private readonly UygulamaDbContext _context;

        public UyesController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: Uyes
        public async Task<IActionResult> Index()
        {
            return View(await _context.Uyeler.ToListAsync());
        }

        // GET: Uyes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var uye = await _context.Uyeler.FirstOrDefaultAsync(m => m.Id == id);
            if (uye == null) return NotFound();

            return View(uye);
        }

        // GET: Uyes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Uyes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdSoyad,Email,Sifre,Meslek,KanGrubu,Telefon,KayitTarihi,AktifMi,ProfilFotografi")] Uye uye, IFormFile? resimDosyasi)
        {
            if (ModelState.IsValid)
            {
                if (resimDosyasi != null && resimDosyasi.Length > 0)
                {
                    string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                    string kayitYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uyeler", benzersizIsim);

                    var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uyeler");
                    if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                    using (var stream = new FileStream(kayitYolu, FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(stream);
                    }

                    uye.ProfilFotografi = "/img/uyeler/" + benzersizIsim;
                }

                // 🚨 BANKA DÜZEYİ GÜVENLİK: Üyeyi kaydetmeden hemen önce şifresini kriptoluyoruz! 🚨
                uye.Sifre = Sifreleme.HashSifre(uye.Sifre);

                _context.Add(uye);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uye);
        }

        // GET: Uyes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var uye = await _context.Uyeler.FindAsync(id);
            if (uye == null) return NotFound();

            return View(uye);
        }

        // POST: Uyes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyad,Email,Sifre,Meslek,KanGrubu,Telefon,KayitTarihi,AktifMi,ProfilFotografi")] Uye uye, IFormFile? resimDosyasi)
        {
            if (id != uye.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (resimDosyasi != null && resimDosyasi.Length > 0)
                    {
                        // ESKİ FOTOĞRAFI FİZİKSEL KLASÖRDEN SİL
                        if (!string.IsNullOrEmpty(uye.ProfilFotografi))
                        {
                            string eskiFizikselYol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uye.ProfilFotografi.TrimStart('/'));
                            if (System.IO.File.Exists(eskiFizikselYol))
                            {
                                System.IO.File.Delete(eskiFizikselYol);
                            }
                        }

                        // YENİ FOTOĞRAFI YÜKLE
                        string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                        string kayitYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uyeler", benzersizIsim);

                        var klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/uyeler");
                        if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                        using (var stream = new FileStream(kayitYolu, FileMode.Create))
                        {
                            await resimDosyasi.CopyToAsync(stream);
                        }

                        uye.ProfilFotografi = "/img/uyeler/" + benzersizIsim;
                    }

                    // 🚨 DÜZENLEME EKRANI GÜVENLİĞİ: 
                    // Eğer şifre alanı doluysa ve 64 karakterden kısaysa (Yani SHA256 değilse, admin yeni bir şifre yazmış demektir) onu da kriptola!
                    if (!string.IsNullOrEmpty(uye.Sifre) && uye.Sifre.Length != 64)
                    {
                        uye.Sifre = Sifreleme.HashSifre(uye.Sifre);
                    }

                    _context.Update(uye);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UyeExists(uye.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(uye);
        }

        // GET: Uyes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var uye = await _context.Uyeler.FirstOrDefaultAsync(m => m.Id == id);
            if (uye == null) return NotFound();

            return View(uye);
        }

        // POST: Uyes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uye = await _context.Uyeler.FindAsync(id);
            if (uye != null)
            {
                // ÜYEYİ SİLMEDEN HEMEN ÖNCE FOTOĞRAFINI DA KLASÖRDEN SİL
                if (!string.IsNullOrEmpty(uye.ProfilFotografi))
                {
                    string fizikselYol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uye.ProfilFotografi.TrimStart('/'));
                    if (System.IO.File.Exists(fizikselYol))
                    {
                        System.IO.File.Delete(fizikselYol);
                    }
                }

                _context.Uyeler.Remove(uye);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // EXCEL RAPORU ALMA METODU 📗
        public async Task<IActionResult> ExcelIndir()
        {
            var uyeler = await _context.Uyeler.ToListAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Üye Listesi");

                worksheet.Cell(1, 1).Value = "Ad Soyad";
                worksheet.Cell(1, 2).Value = "E-Posta";
                worksheet.Cell(1, 3).Value = "Telefon";
                worksheet.Cell(1, 4).Value = "Meslek";
                worksheet.Cell(1, 5).Value = "Kan Grubu";
                worksheet.Cell(1, 6).Value = "Kayıt Tarihi";
                worksheet.Cell(1, 7).Value = "Durum";
                worksheet.Row(1).Style.Font.Bold = true;

                int row = 2;
                foreach (var uye in uyeler)
                {
                    worksheet.Cell(row, 1).Value = uye.AdSoyad;
                    worksheet.Cell(row, 2).Value = uye.Email;
                    worksheet.Cell(row, 3).Value = uye.Telefon;
                    worksheet.Cell(row, 4).Value = uye.Meslek;
                    worksheet.Cell(row, 5).Value = uye.KanGrubu;
                    worksheet.Cell(row, 6).Value = uye.KayitTarihi.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 7).Value = uye.AktifMi ? "Aktif" : "Pasif";
                    row++;
                }

                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Dernek_Uyeleri.xlsx");
                }
            }
        }

        private bool UyeExists(int id)
        {
            return _context.Uyeler.Any(e => e.Id == id);
        }
    }
}