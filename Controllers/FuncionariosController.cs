using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
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
    [Authorize(Roles = Statics.Roles.GESTOR)]
    public class FuncionariosController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        public FuncionariosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }


        // GET: Imoveis
        public async Task<IActionResult> Index()
        {
            var helper = new HelperClass(this);
            var applicationDbContext = _context.Users.Where(c => c.GestorId == helper.userId);
            var users = await applicationDbContext.ToListAsync();
            ViewBag.helper = helper;
            return View(users);
        }

        private object Helper(ref ImoveisController imoveisController)
        {
            throw new NotImplementedException();
        }

        // GET: Imoveis/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Imoveis/Create
        public IActionResult Create()
        {
            return View();
        }




        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            Create([Bind("Email,Password")] NewUser newUser)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = newUser.Email,
                    Email = newUser.Email,
                    GestorId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                };
                var result_creation = await _userManager.CreateAsync(user, newUser.Password);
                if (result_creation.Succeeded)
                {
                    var result_addrole = await _userManager.AddToRoleAsync(user, Statics.Roles.FUNCIONARIO);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result_creation.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }

        public class NewUser
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

        }

        // GET: Imoveis/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        // POST: Imoveis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.GestorId != HelperClass.getUserId(this))
            {
                return NotFound();
            }

            var editUser = new EditUser();
            editUser.Email = user.Email;

            return View(editUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ChangeEmail,Email,ChangePassword,Password")] EditUser user_new_info)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user.GestorId != HelperClass.getUserId(this))
                {
                    ModelState.AddModelError(string.Empty, "This id is not valid to edit.");
                }
                //changing email
                if (user_new_info.ChangeEmail)
                {
                    if (string.IsNullOrEmpty(user_new_info.Email))
                    {
                        ModelState.AddModelError(string.Empty, "There is no email to change.");
                        return View(user_new_info);
                    }

                    if (user.Email != user_new_info.Email)
                    {
                        var result = await _userManager.ChangeEmailAsync(
                                                    user, user_new_info.Email,
                                                    await _userManager.GenerateChangeEmailTokenAsync(user, user_new_info.Email)
                                                );
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);

                            }
                            return View(user_new_info);
                        }
                    }
                }
                if (user_new_info.ChangePassword)
                {
                    //if errors
                    if (string.IsNullOrEmpty(user_new_info.Password))
                    {
                        ModelState.AddModelError(string.Empty, "There is no password to change.");
                        return View(user_new_info);
                    }

                    var result_password_change =
                    await _userManager.ChangePasswordAsync(
                        user, user_new_info.Password,
                        await _userManager.GeneratePasswordResetTokenAsync(user)
                    );
                    if (!result_password_change.Succeeded)
                    {
                        foreach (var error in result_password_change.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(user_new_info);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(user_new_info);

        }

        public class EditUser
        {

            [EmailAddress]
            public string Email { get; set; }


            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [Display(Name = "Mudar Password")]
            public bool ChangePassword { get; set; }
            [Required]
            [Display(Name = "Mudar Email")]
            public bool ChangeEmail { get; set; }
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

