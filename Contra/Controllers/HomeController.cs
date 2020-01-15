using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Contra.Areas.Identity.Data;
using Contra.Data;
using Contra.Models;

namespace Contra.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ContraUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public HomeController(ApplicationDbContext context,
                              UserManager<ContraUser> userManager,
                              IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public IActionResult Index()
        {
            Article placeholder = new Article
            {
                Title = "Relevant Story",
                SummaryLong = "Oops! Looks like we don't have enough articles to show you right now.",
                Id = -1
            };

            List<Article> articles = (from a in _context.Article
                                      where a.Approved == ApprovalStatus.Approved &&
                                            a.ArticleType == ArticleType.Article && 
                                            a.Date >= DateTime.Now.AddDays(-2)
                                      orderby a.Views descending
                                      select a).ToList();

            Random rnd = new Random();
            while (articles.Count < 5)
            {
                placeholder.ThumbnailURL = "/img/img0" + rnd.Next(1, 5).ToString() + ".jpg";
                articles.Add(placeholder);
            }

            return View(articles);
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comment([Bind("Id,Content")] Comment comment, int PostId)
        {
            if (ModelState.IsValid)
            {
                ContraUser user = _userManager.GetUserAsync(User).Result;
                comment.OwnerID = _userManager.GetUserId(User);
                comment.AuthorName = user.Name;
                comment.PostId = PostId;
                comment.Date = DateTime.Now;
                
                if (User.IsInRole("Staff"))
                    comment.Approved = ApprovalStatus.Approved;
                else
                    comment.Approved = ApprovalStatus.Submitted;

                if (user.Comments == null)
                    user.Comments = new List<Comment>();
                user.Comments.Add(comment);

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return Redirect($"~/article/{PostId}");
            }
            return View(comment);
        }

        [HttpPost("/Home/Search")]
        public IActionResult Search(string filter)
        {
            if (string.IsNullOrEmpty(filter)) filter = "all";
            return Redirect($"/search/{filter}");
        }

        [HttpGet("/search/{filter}/{*sortBy}")]
        public IActionResult Search(string filter, string sortBy = "trending")
        {
            if (string.IsNullOrEmpty(filter)) filter = "all";
            ViewData["Filter"] = filter;

            List<Article> articles;
            switch (filter.ToLower())
            {
                case "all":
                    ViewData["Message"] = "Search All";
                    articles = (from a in _context.Article
                                where a.Approved == ApprovalStatus.Approved
                                orderby a.Date descending
                                select a).ToList();
                    break;
                default:
                    ViewData["Message"] = "Search - " + filter;
                    articles = (from a in _context.Article
                                where a.Approved == ApprovalStatus.Approved &&
                                      (a.Title.ToLower().Contains(filter.ToLower()) || 
                                       a.Tags.ToLower().Contains(filter.ToLower()) || 
                                       a.SummaryLong.ToLower().Contains(filter.ToLower()))
                                select a).ToList();
                    break;
            }

            switch (sortBy)
            {
                case "author":
                    ViewData["SortBy"] = "Author";
                    articles = articles.OrderBy(a => a.AuthorName).ToList();
                    break;
                case "new":
                    ViewData["SortBy"] = "New";
                    articles = articles.OrderByDescending(a => a.Date).ToList();
                    break;
                case "top":
                    ViewData["SortBy"] = "Top";
                    articles = articles.OrderBy(a => a.Views).ToList();
                    break;
                default:
                    ViewData["SortBy"] = "Trending";
                    articles = articles.OrderBy(a => a.Views).ToList();
                    break;
            }

            ViewData["Query"] = filter;
            return View(articles);
        }

        [HttpGet("/submit")]
        public IActionResult Submit()
        {
            return View();
        }

        [Authorize]
        [HttpPost("/submit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([Bind("Id,ArticleType,AuthorName,Title,Sensitive," +
                                                      "SensitiveContent,Spoiler,SpoilerContent,Anonymous," +
                                                      "Tags,SummaryLong,Content")] Article article, IFormFile thumbnail)
        {
            if (ModelState.IsValid)
            {
                ContraUser user = _userManager.GetUserAsync(User).Result;
                if (thumbnail.Length > 0 && thumbnail.Length < 2000000)
                {
                    string rootPath = _environment.WebRootPath + "/img/user/" + user.Id;
                    if (!Directory.Exists(rootPath))
                        Directory.CreateDirectory(rootPath);

                    string filePath = rootPath + "/" + Path.GetRandomFileName();
                    switch (thumbnail.ContentType)
                    {
                        case "image/png":
                            filePath += ".png";
                            break;
                        case "image/jpeg":
                            filePath += ".jpg";
                            break;
                        default:
                            return View(article);
                    }

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await thumbnail.CopyToAsync(stream);
                    }

                    article.ThumbnailURL = filePath;
                }
                else
                    return View(article);

                article.OwnerID = user.Id;
                if (!string.IsNullOrWhiteSpace(article.AuthorName))
                    article.AuthorName = user.Name + ", " + article.AuthorName;
                else
                    article.AuthorName = user.Name;
                article.Date = DateTime.Now;
                article.Views = 0;

                if (User.IsInRole("Staff"))
                {
                    article.Approved = ApprovalStatus.Approved;
                    article.IsEditorial = true;
                }
                else
                    article.Approved = ApprovalStatus.Submitted;

                if (user.Articles == null)
                    user.Articles = new List<Article>();
                user.Articles.Add(article);

                _context.Add(article);
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

        [Route("/about")]
        public IActionResult About()
        {
            return View(_userManager.GetUsersInRoleAsync("Staff").Result);
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
