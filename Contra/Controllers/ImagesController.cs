using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Contra.Data;
using Contra.Models;

namespace Contra.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            return View(await _context.Image.ToListAsync());
        }

        [HttpGet("/img/upload/{id}")]
        public FileStreamResult GetImage(int id)
        {
            var image = (from i in _context.Image
                         where i.Id == id
                         select i).FirstOrDefault();

            Stream stream = new MemoryStream(image.Content);
            return new FileStreamResult(stream, image.ContentType);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                        ContentType = file.ContentType,
                        Content = memoryStream.ToArray()
                    };

                    _context.Image.Add(image);

                    await _context.SaveChangesAsync();
                }
            }

            return View();
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Image.FindAsync(id);
            _context.Image.Remove(image);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Image.Any(e => e.Id == id);
        }
    }
}
