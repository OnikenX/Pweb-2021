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
            var applicationDbContext = _context.Feedbacks.Include(f => f.ApplicationUser).Include(f => f.Reserva).Include(f => f.Reserva.Imovel);
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
                .Include(f => f.Reserva)
                .Include(f => f.Reserva.Imovel)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }
        [Authorize]
        // GET: Feedbacks/Create
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = new Feedback();
            feedback.ReservaId = (int)id;
            return View(feedback);
        }

        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Estrelas,Comentario,ReservaId")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.ApplicationUserId = HelperClass.getUserId(this);
                
                var reserva = await _context.Reservas.FindAsync(feedback.ReservaId);
                if (reserva == null)
                {
                    ModelState.AddModelError(string.Empty, "A reserva relacionada com este comentario não existe.");
                    return View(feedback);
                }
                feedback.AuthorIsCliente = new HelperClass(this).isCliente;
                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return await VoltarLista(feedback);
            }
            return View(feedback);
        }
        public async Task<IActionResult> VoltarLista(Feedback feedback)
        {
            if (new HelperClass(this).isCliente)
            {
                return RedirectToAction(nameof(Details), "Imoveis", new { id = feedback.Reserva.ImovelId });
            }
            else
            {
                return RedirectToAction("UserDetails", "Reservas", new { id = feedback.Reserva.ApplicationUserId });
            }
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
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Estrelas,Comentario")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var fb_db = await _context.Feedbacks.FindAsync(id);
                    if(fb_db == null)
                    {
                        return NotFound();
                    }

                    if (!FeedbackDoUser(fb_db))
                    {
                        return NotFound();
                    }

                    fb_db.Estrelas = feedback.Estrelas;
                    fb_db.Comentario= feedback.Comentario;

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
                return await VoltarLista(feedback);
            }   
            return View(feedback);
        }

        [Authorize(Roles = Statics.Roles.FUNCIONARIO_CLIENTE)]
        // GET: Feedbacks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var feedback= await _context.Feedbacks.FindAsync(id);
            
            if (feedback == null)
            {
                return NotFound();
            }
            feedback.Reserva = await _context.Reservas.FindAsync(feedback.ReservaId);
            feedback.ApplicationUser = await _context.Users.FindAsync(feedback.ApplicationUserId);
            if (feedback.Reserva != null)
            {
                feedback.Reserva.Imovel = await _context.Imoveis.FindAsync(feedback.Reserva.ImovelId);
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            if (!FeedbackDoUser(feedback))
            {
                return NotFound();
            }
            feedback.Reserva = await _context.Reservas.FindAsync(feedback.ReservaId);
            feedback.ApplicationUser = await _context.Users.FindAsync(feedback.ApplicationUserId);
            if (feedback.Reserva != null)
            {
                feedback.Reserva.Imovel = await _context.Imoveis.FindAsync(feedback.Reserva.ImovelId);
            }
            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "Imoveis", new { id = feedback.Reserva.ImovelId });
        }


        //verifica se o user tem permissoes sobre o item
        private bool FeedbackDoUser(Feedback Feedback)
        {
            var helper = new HelperClass(this);
            if (helper.isCliente)
            {
                return helper.userId == Feedback.ApplicationUserId;

            }
            else if (helper.isAdmin)
            {
                return true;
            }
            else if (helper.isFunc)
            {
                return Feedback.ApplicationUserId == helper.userId;
            }
            else if (helper.isGestor)
            {
                return Feedback.ApplicationUser.GestorId == helper.userId || Feedback.ApplicationUserId == helper.userId;
            }
            return false;
        }

        private bool FeedbackExists(int id)
        {
            return _context.Feedbacks.Any(e => e.FeedbackId == id);
        }
    }
}
