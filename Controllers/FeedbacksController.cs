using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pweb_2021.Data;
using Pweb_2021.Models;

namespace Pweb_2021.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Feedbacks.Include(f => f.ApplicationUser).Include(f => f.Imovel);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.ApplicationUser)
                .Include(f => f.Imovel)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = new Feedback();
            feedback.ImovelId = (int)id;
            //ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id");
            //ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId");
            return View(feedback);
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = Statics.Roles.CLIENTE)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Estrelas,Comentario,ImovelId")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.ApplicationUserId = HelperClass.getUserId(this);
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), "Imoveis", new { id = feedback.ImovelId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", feedback.ApplicationUserId);
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", feedback.ImovelId);
            return View(feedback);
        }

        // GET: Feedbacks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", feedback.ApplicationUserId);
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", feedback.ImovelId);
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,Estrelas,Comentario,ApplicationUserId,ImovelId")] Feedback feedback)
        {
            if (id != feedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.FeedbackId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), "Imoveis", new { id = feedback.ImovelId });
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", feedback.ApplicationUserId);
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", feedback.ImovelId);
            return View(feedback);
        }

        [Authorize(Roles = Statics.Roles.CLIENTE)]
        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.ApplicationUser)
                .Include(f => f.Imovel)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [Authorize(Roles = Statics.Roles.CLIENTE)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "Imoveis", new {id = feedback.ImovelId});
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackId == id);
        }
    }
}
