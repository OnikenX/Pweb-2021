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
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservasController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool reservaIdsequals(Reserva rs)
        {
            var helper = new HelperClass(this);
            if (helper.isAdmin)
                return true;
            else if (helper.isGestor)
            {
                return rs.Imovel.ApplicationUserId == helper.userId;
            }
            else if (helper.isFunc)
            {
                var userFun = _context.Users.Find(helper.userId);
                if (userFun == null)
                {
                    return false;
                }
                return rs.Imovel.ApplicationUserId == userFun.GestorId;
            }
            else if (helper.isCliente)
            {
                return rs.ApplicationUserId == helper.userId;
            }
            else
            {
                return false;
            }
        }

        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var helper = new HelperClass(this);
            ViewBag.helper = helper;
            var applicationDbContext = await _context.Reservas.Include(r => r.ApplicationUser).Include(r => r.Imovel).ToListAsync();
            var listaReserva = applicationDbContext.Where(rs => reservaIdsequals(rs)).ToList();
            ViewData["reservas"] = listaReserva;
            return View();
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.ApplicationUser)
                .Include(r => r.Imovel)
                .FirstOrDefaultAsync(m => m.ReservaId == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create

        [Authorize(Roles = Statics.Roles.CLIENTE)]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.helper = new HelperClass(this);

            ViewBag.helper.extraId1 = (int)id;
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Statics.Roles.CLIENTE)]
        public async Task<IActionResult> Create([Bind("ReservaId,DataInicial,DataFinal,ImovelId,ApplicationUserId")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                reserva.Estado = 1;
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.helper = new HelperClass(this);
            ViewBag.helper.extraId1 = reserva.ImovelId;
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        [Authorize(Roles = Statics.Roles.ADMIN_GESTOR_FUNCIONARIO)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", reserva.ApplicationUserId);
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", reserva.ImovelId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Statics.Roles.ADMIN_GESTOR_FUNCIONARIO)]
        public async Task<IActionResult> Edit(int id, [Bind("ReservaId,DataInicial,DataFinal,Comentario,Estado,ImovelId,ApplicationUserId")] Reserva reserva)
        {
            if (id != reserva.ReservaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.ReservaId))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.Users, "Id", "Id", reserva.ApplicationUserId);
            ViewData["ImovelId"] = new SelectList(_context.Imoveis, "ImovelId", "ApplicationUserId", reserva.ImovelId);
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Statics.Roles.GESTOR_FUNCIONARIO)]
        public async Task<IActionResult> UpdateEstado([Bind("ReservaId,Estado")] Reserva valor)
        {
            do
            {
                var reserva = await _context.Reservas.FindAsync(valor.ReservaId);
               if (reserva == null)
                {
                    break;
                }
                reserva.Estado = valor.Estado;
                _context.Reservas.Update(reserva);
                _context.SaveChanges();
                break;
            } while (false);
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservas/Delete/5

        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var reserva = await _context.Reservas
        //        .Include(r => r.ApplicationUser)
        //        .Include(r => r.Imovel)
        //        .FirstOrDefaultAsync(m => m.ReservaId == id);
        //    if (reserva == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(reserva);
        //}

        //// POST: Reservas/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var reserva = await _context.Reservas.FindAsync(id);
        //    _context.Reservas.Remove(reserva);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaId == id);
        }
    }
}
