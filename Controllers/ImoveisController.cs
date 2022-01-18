using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pweb_2021.Data;
using Pweb_2021.Models;
using Pweb_2021.ViewModels;

namespace Pweb_2021.Controllers
{
    public class ImoveisController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ImoveisController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            webHostEnvironment = hostEnvironment;
        }

        // GET: Imoveis
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Imoveis.Include(i => i.ApplicationUser);
            var imagens = await _context.ImovelImgs.ToListAsync();
            ViewData["imagens"] = imagens;

            ViewBag.helper = new HelperClass(this);
            return View(await applicationDbContext.ToListAsync());
        }

        private object Helper(ref ImoveisController imoveisController)
        {
            throw new NotImplementedException();
        }

        // GET: Imoveis/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovel = await _context.Imoveis
                .Include(i => i.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ImovelId == id);
            if (imovel == null)
            {
                return NotFound();
            }
            ViewData["comentarios"] = await _context.Feedbacks
                .Where(fb => fb.Reserva.ImovelId == id)
                .Include(fb => fb.ApplicationUser).ToListAsync();
            ViewData["imagens"] = await _context.ImovelImgs
                .Where(img => img.ImovelId == id).ToListAsync();
            ViewBag.helper = new HelperClass(this);
            return View(imovel);
        }

        [Authorize(Roles = Statics.Roles.GESTOR)]
        // GET: Imoveis/Create
        public IActionResult Create()
        {
            ViewBag.helper = new HelperClass(this);
            return View();
        }


        // POST: Imoveis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = Statics.Roles.GESTOR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ImovelId,Nome,Descricao,Preco,ApplicationUserId")] Imovel imovel)
        {
            imovel.ApplicationUser = await _context.Users.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            imovel.ApplicationUserId = imovel.ApplicationUser.Id;
            if (ModelState.IsValid)
            {
                _context.Add(imovel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.helper = new HelperClass(this);
            return View(imovel);
        }


        [Authorize(Roles = Statics.Roles.GESTOR)]
        // GET: Imoveis/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var imovel = await _context.Imoveis.FindAsync(id);
            if (imovel == null)
            {
                return NotFound();
            }
            ViewData["imagens"] = await _context.ImovelImgs.Where(img => img.ImovelId == id).ToListAsync();
            return View(imovel);
        }

        // POST: Imoveis/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = Statics.Roles.GESTOR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImovelId,Nome,Descricao,ApplicationUserId")] Imovel imovel)
        {
            if (id != imovel.ImovelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(imovel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImovelExists(imovel.ImovelId))
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
            ViewData["imagens"] = await _context.ImovelImgs.Where(img => img.ImovelId == id).ToListAsync();
            return View(imovel);
        }

        // GET: Imoveis/Delete/5
        [Authorize(Roles = Statics.Roles.ADMIN_GESTOR)]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var imovel = await _context.Imoveis
                .Include(i => i.ApplicationUser)
                .FirstOrDefaultAsync(m => m.ImovelId == id);
            if (imovel == null)
            {
                return NotFound();
            }

            return View(imovel);
        }

        // POST: Imoveis/Delete/5
        [Authorize(Roles = Statics.Roles.ADMIN_GESTOR)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var imovel = await _context.Imoveis.FindAsync(id);
            var reservas = await _context.Reservas.Where(rs => rs.ImovelId == id).ToListAsync();
            
            foreach(var reserva in reservas)
            {
                var feedbacks = await _context.Feedbacks.Where(fb => fb.ReservaId == reserva.ReservaId).ToListAsync();
                foreach (var feedback in feedbacks)
                {
                    _context.Feedbacks.Remove(feedback);
                }
                _context.Reservas.Remove(reserva);
            }
            var imagens = await _context.ImovelImgs.Where(img => img.ImovelImgId == id).ToListAsync(); 
            foreach(var imagem in imagens)
            {
                DeleteFile(imagem.pathToImage);
                _context.Remove(imagem);
            }
            _context.Imoveis.Remove(imovel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImovelExists(int id)
        {
            return _context.Imoveis.Any(e => e.ImovelId == id);
        }




        ///////////////////////////////
        ///////////////////////////////
        ///////////////////////////////
        /////////// IMAGENS ///////////
        ///////////////////////////////
        ///////////////////////////////
        ///////////////////////////////

        [Authorize(Roles = Statics.Roles.ADMIN_GESTOR)]
        // GET: Imoveis/Create
        public async Task<IActionResult> AddImg(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var helper = new HelperClass(this);
            helper.extraId1 = (int)id;
            ViewBag.helper = helper;



            return View();
        }


        // POST: Imoveis/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = Statics.Roles.GESTOR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddImg([Bind("ImovelImgId,Description,Image,ImovelId")] ImovelImgViewModel model)
        {

            var helper = new HelperClass(this);
            helper.extraId1 = (int)model.ImovelId;
            ViewBag.helper = helper;

            if (ModelState.IsValid)
            {
                var supportedTypes = new[] { "jpg", "png", "webp", "jfif" };
                var fileExt = System.IO.Path.GetExtension(model.Image.FileName).Substring(1);

                if (!supportedTypes.Contains(fileExt))
                {
                    ModelState.AddModelError(string.Empty, $"A imagem só pode ter uma das seguintes extensões: {string.Join(',', supportedTypes)}");
                    return View(model);
                }

                var file_name = UploadedFile(model);
                var image = new ImovelImg
                {
                    ImovelId = model.ImovelId,
                    Description = model.Description,
                    pathToImage = file_name
                };
                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit), new { id = model.ImovelId });
            }

            return View(model);
        }


        [Authorize(Roles = Statics.Roles.ADMIN_GESTOR)]
        // GET: ImovelImgs/Delete/5
        public async Task<IActionResult> DeleteImg(int? id)
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

        // POST: ImovelImgs/Delete/5
        [HttpPost, ActionName("DeleteImg")]
        [Authorize(Roles = Statics.Roles.ADMIN_GESTOR)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImgConfirmed(int id)
        {

            var imovelImg = await _context.ImovelImgs.FindAsync(id);
            var imovel_id = imovelImg.ImovelId;
            DeleteFile(imovelImg.pathToImage);
            _context.ImovelImgs.Remove(imovelImg);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = imovel_id });
        }


        
        private string UploadedFile(ImovelImgViewModel model)
        {
            string uniqueFileName = null;

            if (model.Image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Image.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

       
        //apagar a imagem do fs
        private bool DeleteFile(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                try
                {
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                    string filePath = Path.Combine(uploadsFolder, fileName);
                    System.IO.File.Delete(filePath);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool ImovelImgExists(int id)
        {
            return _context.ImovelImgs.Any(e => e.ImovelImgId == id);
        }
    }
}
