using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DernekSitesi.Models;

namespace DernekSitesi.Controllers
{
    public class ToplantisController : Controller
    {
        private readonly UygulamaDbContext _context;

        public ToplantisController(UygulamaDbContext context)
        {
            _context = context;
        }

        // GET: Toplantis
        public async Task<IActionResult> Index()
        {
            return View(await _context.Toplantilar.ToListAsync());
        }

        // GET: Toplantis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toplanti = await _context.Toplantilar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toplanti == null)
            {
                return NotFound();
            }

            return View(toplanti);
        }

        // GET: Toplantis/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Toplantis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Baslik,Tarih,Konum,GelecekToplantiMi")] Toplanti toplanti)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toplanti);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toplanti);
        }

        // GET: Toplantis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toplanti = await _context.Toplantilar.FindAsync(id);
            if (toplanti == null)
            {
                return NotFound();
            }
            return View(toplanti);
        }

        // POST: Toplantis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Baslik,Tarih,Konum,GelecekToplantiMi")] Toplanti toplanti)
        {
            if (id != toplanti.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toplanti);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToplantiExists(toplanti.Id))
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
            return View(toplanti);
        }

        // GET: Toplantis/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toplanti = await _context.Toplantilar
                .FirstOrDefaultAsync(m => m.Id == id);
            if (toplanti == null)
            {
                return NotFound();
            }

            return View(toplanti);
        }

        // POST: Toplantis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var toplanti = await _context.Toplantilar.FindAsync(id);
            if (toplanti != null)
            {
                _context.Toplantilar.Remove(toplanti);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToplantiExists(int id)
        {
            return _context.Toplantilar.Any(e => e.Id == id);
        }
    }
}
