using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EarthsTimeline.Models;

namespace EarthsTimeline.Controllers
{
    public class CommentsController : Controller
    {
        private readonly TimelineContext _context;

        public CommentsController(TimelineContext context)
        {
            _context = context;
        }

        private bool LoggedIn()
        {
            if (Request.Cookies.ContainsKey("AntiForge") &&
                Request.Cookies["AntiForge"] == "UUDDLRLRBABAS")
                return true;
            else
                return false;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            if (!LoggedIn()) return Redirect("~/login");
            return View(await _context.Comment.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!LoggedIn()) return Redirect("~/login");
            if (id == null) return NotFound();

            var comment = await _context.Comment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null) return NotFound();

            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!LoggedIn()) return Redirect("~/login");
            if (id == null) return NotFound();

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null) return NotFound();

            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Approved,PostId,Date,AuthorId,AuthorName,Content")] Comment comment)
        {
            if (!LoggedIn()) return Redirect("~/login");
            if (id != comment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(comment);
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
