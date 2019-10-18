using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EarthsTimeline.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace EarthsTimeline.Controllers
{
    public class HomeController : Controller
    {
        private readonly TimelineContext _context;

        public HomeController(TimelineContext context)
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

        [Route("/logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AntiForge");
            return Redirect("~/");
        }

        [Route("/login")]
        [HttpGet]
        public IActionResult Login()
        {
            if (LoggedIn()) return Redirect("~/admin");
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public IActionResult Login(string User)
        {
            if (User == "epstein, please no")
            {
                Response.Cookies.Append("AntiForge", "UUDDLRLRBABAS", 
                        new CookieOptions() { Path = "/", Expires = DateTime.Now.AddDays(1), IsEssential = true } );
                return Redirect("~/admin");
            }
            else return View();
        }

        [Route("/admin")]
        public async Task<IActionResult> Admin()
        {
            if (!LoggedIn()) return Redirect("~/login");

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

        public IActionResult ApproveArticle(int id)
        {
            if (!LoggedIn()) return Redirect("~/");

            Article article = _context.Article.Where(x => x.Id == id).FirstOrDefault();
            if (article != null)
            {
                article.Approved = true;
                _context.SaveChanges();
            }

            return Redirect("~/admin");
        }

        public IActionResult ApproveComment(int id)
        {
            if (!LoggedIn()) return Redirect("~/");

            Comment comment = _context.Comment.Where(x => x.Id == id).FirstOrDefault();
            if (comment != null)
            {
                comment.Approved = true;
                _context.SaveChanges();
            }

            return Redirect("~/admin");
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
                                 where a.Approved == true
                                 orderby a.Date descending
                                 select a).ToList();
            while (top.Count < 4)
            {
                placeholder.ThumbnailURL = "../img/img0" + rnd.Next(1, 5).ToString() + ".png";
                top.Add(placeholder);
            }
            articles.Add(top);

            List<Article> editorial = (from a in _context.Article
                                       where a.Approved == true && 
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

            List<Comment> comments = await (from c in _context.Comment
                                            where c.PostId == article.Id
                                            && c.Approved
                                            orderby c.Date descending
                                            select c).ToListAsync();
            ViewData["Comments"] = comments;
            ViewData["PendingComments"] = (await (from c in _context.Comment
                                                  where c.PostId == article.Id
                                                  && !c.Approved
                                                  select c).ToListAsync()).Count;

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

        [Route("/search/{*param}")]
        public IActionResult Search(string param)
        {
            if (string.IsNullOrEmpty(param))
                return View((from a in _context.Article
                             where a.Approved
                             orderby a.Date descending
                             select a).ToList());

            ViewData["Query"] = "- " + param;
            param = param.ToLower();
            return View((from a in _context.Article
                         where a.Approved
                         && (a.Title.ToLower().Contains(param)
                         || a.SummaryShort.ToLower().Contains(param))
                         orderby a.Date descending
                         select a).ToList());
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
                if (article.SummaryLong.Length > 60) 
                    article.SummaryLong = article.SummaryLong.Substring(0, 60) + "...";
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
