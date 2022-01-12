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
        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }


        // GET: Imoveis
        public async Task<IActionResult> Index()
        {
            var helper = new HelperClass(this);
            var  userRoles =await  _context.UserRoles.ToListAsync();
            ViewData["userRoles"] = userRoles;
            var roles = await _context.Roles.ToListAsync();
            ViewData["roles"] = roles;
            var users = await _context.Users.ToListAsync();
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

        // GET: Users/Delete/5
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


        // POST: Users/Delete/5
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

            var editUser = new EditUser();
            editUser.Email = user.Email;

            return View(editUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("UserName,Email,ChangePassword,ChangeEmail")] EditUser user_new_info)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
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

