using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pweb_2021.Data;
using Pweb_2021.Models;

namespace Pweb_2021.Controllers
{
    public class ImovelImgsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImovelImgsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ImovelImgs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ImovelImgs.Include(i => i.Imovel);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ImovelImgs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovelImg = await _context.ImovelImgs
                .Include(i => i.Imovel)
                .FirstOrDefaultAsync(m => m.ImovelImgId == id);
            if (imovelImg == null)
            {
                return NotFound();
            }

            return View(imovelImg);
        }

        // GET: ImovelImgs/Create
        public IActionResult Create()
        {
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId");
            return View();
        }

        // POST: ImovelImgs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImovelImgId,Description,Image,ImovelId")] ImovelImg imovelImg)
        {
            if (ModelState.IsValid)
            {
                _context.Add(imovelImg);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", imovelImg.ImovelId);
            return View(imovelImg);
        }

        // GET: ImovelImgs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovelImg = await _context.ImovelImgs.FindAsync(id);
            if (imovelImg == null)
            {
                return NotFound();
            }
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", imovelImg.ImovelId);
            return View(imovelImg);
        }

        // POST: ImovelImgs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImovelImgId,Description,Image,ImovelId")] ImovelImg imovelImg)
        {
            if (id != imovelImg.ImovelImgId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(imovelImg);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImovelImgExists(imovelImg.ImovelImgId))
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
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", imovelImg.ImovelId);
            return View(imovelImg);
        }

        // GET: ImovelImgs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovelImg = await _context.ImovelImgs
                .Include(i => i.Imovel)
                .FirstOrDefaultAsync(m => m.ImovelImgId == id);
            if (imovelImg == null)
            {
                return NotFound();
            }

            return View(imovelImg);
        }

        // POST: ImovelImgs/DeleteI/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var imovelImg = await _context.ImovelImgs.FindAsync(id);
            _context.ImovelImgs.Remove(imovelImg);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImovelImgExists(int id)
        {
            return _context.ImovelImgs.Any(e => e.ImovelImgId == id);
        }
    }
}
