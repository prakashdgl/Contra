using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTalon.Data;
using OpenTalon.Models;

namespace OpenTalon.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
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

        public async Task<IActionResult> Index()
        {
            if (!LoggedIn()) return Redirect("~/login");
            return View(await _context.Article.ToListAsync());
        }

        [Route("/articlemanager/{*id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (!LoggedIn()) return Redirect("~/login");
            if (id == null) return NotFound();

            var article = await _context.Article
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null) return NotFound();

            List<Comment> comments = await (from c in _context.Comment
                                            where c.PostId == article.Id
                                            && c.Approved
                                            orderby c.Date descending
                                            select c).ToListAsync();
            ViewData["Comments"] = comments;
            ViewData["PendingComments"] = (await (from c in _context.Comment
                                                  where c.PostId == article.Id
                                                  && !c.Approved
                                                  orderby c.Date descending
                                                  select c).ToListAsync()).Count;

            return View(article);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!LoggedIn()) return Redirect("~/login");
            if (id == null) return NotFound();

            var article = await _context.Article.FindAsync(id);
            if (article == null) return NotFound();

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Approved,AuthorId,AuthorName,ThumbnailURL,Title,Date,SummaryShort,SummaryLong,Content")] Article article)
        {
            if (!LoggedIn()) return Redirect("~/login");
            if (id != article.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
