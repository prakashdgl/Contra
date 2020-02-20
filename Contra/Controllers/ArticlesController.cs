using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Contra.Data;
using Contra.Models;

namespace Contra.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Article.FindAsync(id);
            if (article == null) return NotFound();

            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ArticleType,ResponseId,Approved,IsArchived,IsPinned,IsEditorial,Sensitive,Anonymous,Spoiler,Views,OwnerID,AuthorName,ThumbnailURL,Title,Date,Tags,SummaryLong,Content")] Article article)
        {
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
                return Redirect("~/articles/all");
            }
            return View(article);
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
