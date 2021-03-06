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
    [Authorize(Roles = Statics.Roles.ADMIN)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ImoveisController _imoveisController;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ImoveisController imoveisController)
        {
            _userManager = userManager;
            _context = context;
            _imoveisController = imoveisController;
        }


        // GET: Imoveis
        public async Task<IActionResult> Index()
        {
            var helper = new HelperClass(this);
            var userRoles = await _context.UserRoles.ToListAsync();
            ViewData["userRoles"] = userRoles;
            var roles = await _context.Roles.ToListAsync();
            ViewData["roles"] = roles;
            var tmpusers = await _context.Users.Where(usr => !usr.Deleted).ToListAsync();
            var users = new List<ApplicationUser>();
            foreach (var user in tmpusers)
            {
                if (!await _userManager.IsInRoleAsync(user, Statics.Roles.ADMIN))
                {
                    users.Add(user);
                }
            }
            ViewBag.helper = helper;
            return View(users);
        }

        private object Helper(ref ImoveisController imoveisController)
        {
            throw new NotImplementedException();
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string? id)
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

            if (await _userManager.IsInRoleAsync(user, Statics.Roles.ADMIN))
            {
                return NotFound();
            }

            ViewData["isGestor"] = await _userManager.IsInRoleAsync(user, Statics.Roles.GESTOR);
            return View(user);
        }


        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                return NotFound();
            }

            user.Deleted = true;
            var imoveis = await _context.Imoveis.Where(imv => imv.ApplicationUserId == user.Id).ToListAsync();


            //apagar os imoveis relacionados
            foreach (var imovel in await _context.Imoveis.Where(imv => imv.ApplicationUserId == id).ToListAsync())
            {
                await _imoveisController.DeleteConfirmed(imovel.ImovelId);
            }

            //apagar os funcionarios relacionados
            foreach (var func in await _context.Users.Where(user => user.GestorId == id).ToListAsync())
            {
                func.Deleted = true;
                _context.Users.Update(func);
            }

            _context.Users.Update(user);
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

            var editUser = new EditUser();
            editUser.Email = user.Email;

            return View(editUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Email,Password,ChangePassword,ChangeEmail")] EditUser user_new_info)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                //changing email
                if (user_new_info.ChangeEmail)
                {
                    if (string.IsNullOrEmpty(user_new_info.Email))
                    {
                        ModelState.AddModelError(string.Empty, "Não existe email par mudar.");
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
                        ModelState.AddModelError(string.Empty, "Não existe password para mudar.");
                        return View(user_new_info);
                    }

                    var result_password_change =
                      await _userManager.ResetPasswordAsync(user,
                                      await _userManager.GeneratePasswordResetTokenAsync(user),
                                      user_new_info.Password
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

