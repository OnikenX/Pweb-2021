using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        public ReservasController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        //detalhes de um user como as suas reviews e tal
        public async Task<IActionResult> UserDetails(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _userManager.FindByIdAsync((string)id);
            if (user == null)
            {
                return NotFound();
            }

            var comentarios = await _context.Feedbacks.Include(fb => fb.Reserva)
                .Include(fb => fb.Reserva.Imovel).Include(fb => fb.ApplicationUser)
                .Where(fb => fb.Reserva.ApplicationUserId == id).ToListAsync();
            ViewData["comentarios"] = comentarios;
            ViewBag.helper = new HelperClass(this);
            return View(user);
        }

        private async Task<bool> UserIsCliente(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var isCliente = true;
            if (!roles.Contains(Statics.Roles.CLIENTE))
            {
                isCliente = false;
            }
            return isCliente;

        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var helper = new HelperClass(this);
            ViewBag.helper = helper;

            var applicationDbContext = await _context.Reservas.Include(rs => rs.Feedbacks).Include(r => r.ApplicationUser).Include(r => r.Imovel).ToListAsync();
            var listaReserva = applicationDbContext.Where(rs => reservaIdsequals(rs)).ToList();

            //verificar quais as reservas que n tem reservas
            var nofeedyet = new List<int>();
            foreach (var reserva in listaReserva)
            {
                if (reserva.Feedbacks.Count >= 2)
                {
                    continue;
                }
                else if (reserva.Feedbacks.Count == 0)
                {
                    nofeedyet.Add(reserva.ReservaId);
                    continue;
                }

                var userid = reserva.Feedbacks[0].ApplicationUserId;
                if (helper.userId == userid)
                {
                    continue;
                }

                var roles = await _context.UserRoles.Where(ur => ur.UserId == userid).ToListAsync();
                string[] roles_superiores = { Statics.Roles.GESTOR, Statics.Roles.FUNCIONARIO, Statics.Roles.ADMIN };
                var anyRoleIsClient = await AnyRoleIsClient(roles);

                if (helper.isCliente)
                {
                    if (!anyRoleIsClient)
                    {
                        nofeedyet.Add(reserva.ReservaId);
                    }
                }
                else
                {
                    if (anyRoleIsClient)
                    {
                        nofeedyet.Add(reserva.ReservaId);
                    }
                }
            }

            ViewData["nofeedyet"] = nofeedyet;
            ViewData["reservas"] = listaReserva;
            return View();
        }

        private async Task<bool> AnyRoleIsClient(List<IdentityUserRole<string>> roles)
        {
            foreach (var role in roles)
            {
                var userRole = await _context.Roles.FindAsync(role.RoleId);
                if (userRole.Name == Statics.Roles.CLIENTE)
                {
                    return true;
                }
            }
            return false;
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
            do
            {
                if (!ModelState.IsValid)
                {
                    break;
                }
                var time_span = (reserva.DataFinal - reserva.DataInicial);
                if (time_span.TotalDays < 0)
                {
                    ModelState.AddModelError(string.Empty, "Não podes fazer escolher uma data final anterior à data inicial.");
                    break;
                }

                if (reserva.DataFinal == reserva.DataInicial)
                {
                    ModelState.AddModelError(string.Empty, "Não podes marcar para o mesmo dia, tem de ser pelo menos 2 dias.");
                    break;
                }

                var now = DateTime.Now;
                if (reserva.DataFinal < now || reserva.DataInicial  < now){
                    ModelState.AddModelError(string.Empty, "Um pouco para o impossivel marcar para uma data no passado, tenta uma data apartir de hoje.");
                    break;
                }

                var reservas = await _context.Reservas.Where(r => r.ImovelId == reserva.ImovelId && reserva.Estado > 1).ToListAsync();
                
                foreach (var outra_reserva in reservas)
                {
                    if (reserva.DataFinal <= outra_reserva.DataFinal && reserva.DataFinal >= outra_reserva.DataInicial ||
                        reserva.DataInicial <= outra_reserva.DataFinal && reserva.DataInicial>= outra_reserva.DataInicial)
                    {
                        ModelState.AddModelError(string.Empty, $"Este estaço de tempo está ocupado, tenta algo antes de {outra_reserva.DataInicial} ou depois de {outra_reserva.DataFinal}.");
                        goto Failure;
                    }
                }

                reserva.Estado = 1;
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            } while (false);
            
            Failure:

            ViewBag.helper = new HelperClass(this);
            ViewBag.helper.extraId1 = reserva.ImovelId;
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
                if(valor.Estado == 3)
                {
                    if(await _context.Reservas.AnyAsync(rs => rs.ImovelId == reserva.ImovelId && rs.Estado == 3))
                    {
                        ModelState.AddModelError(string.Empty, "Este imóvel já tem alguem com a sua posse.");
                        break;
                    }
                }

                reserva.Estado = valor.Estado;
                _context.Reservas.Update(reserva);
                _context.SaveChanges();
                break;
            } while (false);
            return RedirectToAction(nameof(Index));
        }

        //GET: Reservas/Delete/5
        [Authorize(Roles = Statics.Roles.ADMIN)]
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Statics.Roles.ADMIN)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var reserva = await _context.Reservas.FindAsync(id);
            var feedbacks = await _context.Feedbacks.Where(fb => fb.ReservaId == reserva.ReservaId).ToListAsync();
            foreach (var feedback in feedbacks)
            {
                _context.Feedbacks.Remove(feedback);
            }
            _context.Reservas.Remove(reserva);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.ReservaId == id);
        }
    }
}
