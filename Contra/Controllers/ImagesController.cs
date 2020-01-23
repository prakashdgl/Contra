using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Contra.Data;
using Contra.Models;
using Contra.Areas.Identity.Data;

namespace Contra.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ContraUser> _userManager;

        public ImagesController(ApplicationDbContext context,
                                UserManager<ContraUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet("/img/upload/{name}")]
        [ResponseCache(Duration = 604800)]
        public FileStreamResult GetImage(string name)
        {
            var image = (from i in _context.Image
                         where i.Name == name
                         select i).FirstOrDefault();

            if (image == null) return null;

            Stream stream = new MemoryStream(image.Content);
            return new FileStreamResult(stream, image.ContentType);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file)
        {
            if (ModelState.IsValid)
            {
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                // Upload the file if less than 2 MB
                if (memoryStream.Length < 2097152)
                {
                    var image = new Image()
                    {
                        OwnerID = _userManager.GetUserId(User),
                        Name = Path.GetRandomFileName(),
                        ContentType = file.ContentType,
                        Content = memoryStream.ToArray()
                    };

                    _context.Image.Add(image);

                    await _context.SaveChangesAsync();
                }
            }

            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null) return NotFound();

            return View(image);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Image.FindAsync(id);
            _context.Image.Remove(image);
            await _context.SaveChangesAsync();
            return Redirect("/images/all");
        }
    }
}
