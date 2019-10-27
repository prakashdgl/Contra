using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using OpenTalon.Data;
using OpenTalon.Models;

namespace OpenTalon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<List<Article>> articles = new List<List<Article>>();
            Article placeholder = new Article
            {
                Title = "Relevant Story",
                SummaryLong = "Oops! Looks like we don't have enough articles to show you right now.",
                Id = -1
            };

            Random rnd = new Random();
            List<Article> top = (from a in _context.Article
                                 where a.Approved == ApprovalStatus.Approved
                                 orderby a.Date descending
                                 select a).ToList();
            while (top.Count < 4)
            {
                placeholder.ThumbnailURL = "../img/img0" + rnd.Next(1, 5).ToString() + ".png";
                top.Add(placeholder);
            }
            articles.Add(top);

            List<Article> editorial = (from a in _context.Article
                                       where a.Approved == ApprovalStatus.Approved && 
                                       a.SummaryShort.Contains("Featured Editorial")
                                       orderby a.Date descending
                                       select a).ToList();
            while (editorial.Count < 4)
            {
                placeholder.ThumbnailURL = "../img/img0" + rnd.Next(1, 5).ToString() + ".png";
                editorial.Add(placeholder);
            }
            articles.Add(editorial);

            return View(articles);
        }

        [Route("/about")]
        public IActionResult About()
        {
            return View();
        }

        [Route("/article/{*id}")]
        public async Task<IActionResult> Article(int? id)
        {
            if (id == null) return NotFound();

            var article = await _context.Article.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null) return NotFound();

            article.Views++;
            await _context.SaveChangesAsync();

            List<Comment> comments = await (from c in _context.Comment
                                            where c.PostId == article.Id
                                            && c.Approved == ApprovalStatus.Approved
                                            orderby c.Date descending
                                            select c).ToListAsync();
            ViewData["Comments"] = comments;
            ViewData["PendingComments"] = (await (from c in _context.Comment
                                                  where c.PostId == article.Id
                                                  && c.Approved != ApprovalStatus.Approved
                                                  select c).ToListAsync()).Count;

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Comment([Bind("Id,AuthorId,AuthorName,Content")] Comment comment, int PostId)
        {
            if (ModelState.IsValid)
            {
                comment.PostId = PostId;
                comment.Approved = ApprovalStatus.Submitted;
                comment.Date = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return Redirect($"~/article/{comment.PostId}");
            }
            return View(comment);
        }

        [Route("/search/{*param}")]
        public IActionResult Search(string param)
        {
            if (string.IsNullOrEmpty(param))
                return View((from a in _context.Article
                             where a.Approved == ApprovalStatus.Approved
                             orderby a.Date descending
                             select a).Take(8).ToList());

            ViewData["Query"] = param;
            param = param.ToLower();
            return View((from a in _context.Article
                         where a.Approved == ApprovalStatus.Approved
                         && (a.Title.ToLower().Contains(param)
                         || a.SummaryShort.ToLower().Contains(param))
                         orderby a.Date descending
                         select a).Take(8).ToList());
        }

        [HttpGet("/apply")]
        public IActionResult Apply()
        {
            return View();
        }

        [HttpPost("/apply")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply([Bind("Id,AuthorId,AuthorName,ThumbnailURL,Title,SummaryShort,SummaryLong,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                if (article.SummaryLong.Length > 60) 
                    article.SummaryLong = article.SummaryLong.Substring(0, 60) + "...";
                if (string.IsNullOrWhiteSpace(article.ThumbnailURL))
                    article.ThumbnailURL = "../img/img05.png";
                article.Approved = ApprovalStatus.Submitted;
                article.Date = DateTime.Now;
                article.Views = 0;

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
