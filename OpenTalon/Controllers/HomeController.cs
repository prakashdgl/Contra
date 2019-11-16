using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OpenTalon.Areas.Identity.Data;
using OpenTalon.Data;
using OpenTalon.Models;
using Microsoft.AspNetCore.Routing;

namespace OpenTalon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<OpenTalonUser> _userManager;

        public HomeController(ApplicationDbContext context,
                              UserManager<OpenTalonUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                placeholder.ThumbnailURL = "../img/img0" + rnd.Next(1, 5).ToString() + ".jpg";
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
                placeholder.ThumbnailURL = "../img/img0" + rnd.Next(1, 5).ToString() + ".jpg";
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
            if (id == null) return Redirect("~/404");

            var article = await _context.Article.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null) return Redirect("~/404");

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
        public async Task<IActionResult> Comment([Bind("Id,Content")] Comment comment, int PostId)
        {
            if (ModelState.IsValid)
            {
                comment.OwnerID = _userManager.GetUserId(User);
                comment.AuthorName = _userManager.GetUserAsync(User).Result.Name;
                comment.PostId = PostId;
                comment.Approved = ApprovalStatus.Submitted;
                comment.Date = DateTime.Now;

                _context.Add(comment);
                _userManager.GetUserAsync(User).Result.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return Redirect($"~/article/{PostId}");
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
        public async Task<IActionResult> Apply([Bind("Id,AuthorName,ThumbnailURL,Title,SummaryShort,SummaryLong,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrWhiteSpace(article.SummaryLong) || article.SummaryLong.Length > 60) 
                    article.SummaryLong = article.SummaryLong.Substring(0, 60) + "...";
                if (string.IsNullOrWhiteSpace(article.ThumbnailURL))
                    article.ThumbnailURL = "../img/img05.jpg";

                article.OwnerID = _userManager.GetUserId(User);
                article.AuthorName = _userManager.GetUserAsync(User).Result.Name + ", " + article.AuthorName;
                article.Approved = ApprovalStatus.Submitted;
                article.Date = DateTime.Now;
                article.Views = 0;

                _context.Add(article);
                _userManager.GetUserAsync(User).Result.Articles.Add(article);
                await _context.SaveChangesAsync();
                return Redirect("~/success");
            }

            return View(article);
        }

        [HttpGet("/feedback")]
        public IActionResult Feedback()
        {
            return View();
        }

        [HttpPost("/feedback")]
        public async Task<IActionResult> Feedback([Bind("Id,Title,Tags,Content")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(ticket.Title) && string.IsNullOrEmpty(ticket.Content))
                    return View(ticket);

                ticket.OwnerID = _userManager.GetUserId(User);
                ticket.AuthorName = _userManager.GetUserAsync(User).Result.Name;
                ticket.Approved = HandledStatus.Submitted;
                ticket.Date = DateTime.Now;
                ticket.AssignedTo = "None";

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return Redirect("~/success");
            }

            return View(ticket);
        }

        [Route("/privacy")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("/404")]
        public IActionResult ItemNotFound()
        {
            return View();
        }

        [Route("/success")]
        public IActionResult ItemSubmitSuccess()
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
