using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EarthsTimeline.Models;
using Microsoft.EntityFrameworkCore;

namespace EarthsTimeline.Controllers
{
    public class HomeController : Controller
    {
        private readonly TimelineContext _context;

        public HomeController(TimelineContext context)
        {
            _context = context;
        }

        [Route("/admin")]
        public async Task<IActionResult> Admin()
        {
            List<Article> articles = await _context.Article.Where((x) => x.Approved == false).ToListAsync();
            ViewData["Articles"] = _context.Article.Count();
            ViewData["ArticlesList"] = articles;
            ViewData["ArticlesLeft"] = articles.Count;

            List<Comment> comments = await _context.Comment.Where((x) => x.Approved == false).ToListAsync();
            ViewData["Comments"] = _context.Comment.Count();
            ViewData["CommentsList"] = comments;
            ViewData["CommentsLeft"] = comments.Count;

            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("/article/{*id}")]
        public async Task<IActionResult> Article(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Article.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null) return NotFound();

            List<Comment> comments = await _context.Comment.Where(c => c.PostId == article.Id && c.Approved).ToListAsync();
            ViewData["Comments"] = comments;

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Comment([Bind("Id,AuthorId,AuthorName,Content")] Comment comment, int PostId)
        {
            if (ModelState.IsValid)
            {
                comment.PostId = PostId;
                comment.Approved = false;
                comment.Date = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return Redirect($"~/article/{comment.PostId}");
            }
            return View(comment);
        }

        [Route("/search")]
        public async Task<IActionResult> Search()
        {
            return View(await _context.Article.Where((x) => x.Approved == true).ToListAsync());
        }

        [HttpGet("/apply")]
        public IActionResult Apply()
        {
            return View();
        }

        [HttpPost("/apply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("Id,AuthorId,AuthorName,LargeThumbnailURL,SmallThumbnailURL,Title,SummaryShort,SummaryLong,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.Approved = false;
                article.Date = DateTime.Now;

                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }

        [Route("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
