using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
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

        public HomeController(ApplicationDbContext context,
                              UserManager<ContraUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("reconstructing")]
        public IActionResult Reconstructing()
        {
            return View();
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
                                            a.IsArchived != true &&
                                            a.Date >= DateTime.Now.AddDays(-14)
                                      orderby a.Views descending
                                      select a).ToList();

            while (articles.Count < 5)
            {
                placeholder.ThumbnailURL = "/img/img0" + (1 + articles.Count).ToString() + ".jpg";
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

            // Handle view duplication from one user
            if (!Request.Cookies.ContainsKey("VSession"))
            {
                Response.Cookies.Append("VSession", id.ToString() + "-", new CookieOptions
                {
                    MaxAge = TimeSpan.FromHours(1),
                    Secure = true
                });
                article.Views++;
                await _context.SaveChangesAsync();
            }
            else if (!Request.Cookies["VSession"].Contains(id.ToString() + "-"))
            {
                Response.Cookies.Append("VSession", Request.Cookies["VSession"] + id.ToString() + "-", new CookieOptions
                {
                    MaxAge = TimeSpan.FromHours(1),
                    Secure = true
                });
                article.Views++;
                await _context.SaveChangesAsync();
            }

            IEnumerable<Comment> comments = from c in _context.Comment
                                            where c.PostId == article.Id
                                            orderby c.Date descending
                                            select c;
            ViewData["Comments"] = (from c in comments
                                    where c.Approved == ApprovalStatus.Approved
                                    select c).ToList();
            ViewData["PendingComments"] = (from c in comments
                                           where c.Approved != ApprovalStatus.Approved
                                           select c).Count();

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
                if (user.IsBanned) return Redirect("/Identity/Account/Login");

                comment.OwnerID = _userManager.GetUserId(User);
                comment.AuthorName = user.Name;
                comment.PostId = PostId;
                comment.Date = DateTime.Now;
                
                if (User.IsInRole("Staff"))
                    comment.Approved = ApprovalStatus.Approved;
                else
                    comment.Approved = ApprovalStatus.Submitted;

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
                                where a.Approved == ApprovalStatus.Approved &&
                                      a.IsArchived != true &&
                                      a.ArticleType != ArticleType.Blog
                                orderby a.Date descending
                                select a).ToList();
                    break;
                default:
                    ViewData["Message"] = "Search - " + filter;
                    articles = (from a in _context.Article
                                where a.Approved == ApprovalStatus.Approved &&
                                      a.IsArchived != true &&
                                      a.ArticleType != ArticleType.Blog &&
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
                ContraUser user = _userManager.GetUserAsync(User).Result;
                if (user.IsBanned) return Redirect("/Identity/Account/Login");

                if (string.IsNullOrEmpty(ticket.Title) && string.IsNullOrEmpty(ticket.Content))
                    return View(ticket);

                ticket.OwnerID = user.Id;
                ticket.AuthorName = user.Name;
                ticket.Approved = HandledStatus.Submitted;
                ticket.Date = DateTime.Now;
                ticket.AssignedTo = "None";

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return Redirect("~/success");
            }

            return View(ticket);
        }

        [HttpGet("/profile/{*userID}")]
        public async Task<IActionResult> Profile(string userID)
        {
            ContraUser user = await _userManager.FindByIdAsync(userID);
            if (user == null) return Redirect("/404");

            return View(user);
        }

        [Route("/about")]
        public async Task<IActionResult> About()
        {
            return View((await _userManager.GetUsersInRoleAsync("Staff")).OrderBy(u => u.Name));
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
