using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DernekSitesi.Models;
using Microsoft.AspNetCore.Http; // Fotoğraf yüklemek için
using System.IO; // Klasör işlemleri için

namespace DernekSitesi.Controllers
{
    public class YonetimUyesController : Controller
    {
        private readonly UygulamaDbContext _context;

        public YonetimUyesController(UygulamaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Yöneticileri "Sıralama" numarasına göre dizip getiriyoruz (Başkan en üste gelsin diye)
            return View(await _context.YonetimUyeleri.OrderBy(y => y.Siralama).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var yonetimUyesi = await _context.YonetimUyeleri.FirstOrDefaultAsync(m => m.Id == id);
            if (yonetimUyesi == null) return NotFound();
            return View(yonetimUyesi);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // DİKKAT: Buraya resimDosyasi parametresi eklendi
        public async Task<IActionResult> Create([Bind("Id,AdSoyad,Gorev,ResimYolu,TwitterLink,LinkedInLink,InstagramLink,Siralama,Ozgecmis")] YonetimUyesi yonetimUyesi, IFormFile? resimDosyasi)
        {
            if (ModelState.IsValid)
            {
                if (resimDosyasi != null && resimDosyasi.Length > 0)
                {
                    string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                    string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/yonetim");
                    if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);
                    string kayitYolu = Path.Combine(klasorYolu, benzersizIsim);

                    using (var stream = new FileStream(kayitYolu, FileMode.Create))
                    {
                        await resimDosyasi.CopyToAsync(stream);
                    }
                    yonetimUyesi.ResimYolu = "/img/yonetim/" + benzersizIsim;
                }

                _context.Add(yonetimUyesi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(yonetimUyesi);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var yonetimUyesi = await _context.YonetimUyeleri.FindAsync(id);
            if (yonetimUyesi == null) return NotFound();
            return View(yonetimUyesi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AdSoyad,Gorev,ResimYolu,TwitterLink,LinkedInLink,InstagramLink,Siralama,Ozgecmis")] YonetimUyesi yonetimUyesi, IFormFile? resimDosyasi)
        {
            if (id != yonetimUyesi.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (resimDosyasi != null && resimDosyasi.Length > 0)
                    {
                        string benzersizIsim = Guid.NewGuid().ToString() + Path.GetExtension(resimDosyasi.FileName);
                        string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/yonetim");
                        if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);
                        string kayitYolu = Path.Combine(klasorYolu, benzersizIsim);

                        using (var stream = new FileStream(kayitYolu, FileMode.Create))
                        {
                            await resimDosyasi.CopyToAsync(stream);
                        }
                        yonetimUyesi.ResimYolu = "/img/yonetim/" + benzersizIsim;
                    }

                    _context.Update(yonetimUyesi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YonetimUyesiExists(yonetimUyesi.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(yonetimUyesi);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var yonetimUyesi = await _context.YonetimUyeleri.FirstOrDefaultAsync(m => m.Id == id);
            if (yonetimUyesi == null) return NotFound();
            return View(yonetimUyesi);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yonetimUyesi = await _context.YonetimUyeleri.FindAsync(id);
            if (yonetimUyesi != null) _context.YonetimUyeleri.Remove(yonetimUyesi);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool YonetimUyesiExists(int id)
        {
            return _context.YonetimUyeleri.Any(e => e.Id == id);
        }
    }
}