using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTalon.Areas.Identity.Data;
using OpenTalon.Data;
using OpenTalon.Models;

namespace OpenTalon.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<OpenTalonUser> _userManager;

        public AdminController(ApplicationDbContext context,
                               UserManager<OpenTalonUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            List<Article> articles = await _context.Article.Where((x) => 
                                     x.Approved == ApprovalStatus.Submitted).ToListAsync();
            ViewData["Articles"] = _context.Article.Count();
            ViewData["ArticlesLeft"] = articles.Count;

            List<Comment> comments = await _context.Comment.Where((x) => 
                                     x.Approved == ApprovalStatus.Submitted).ToListAsync();
            ViewData["Comments"] = _context.Comment.Count();
            ViewData["CommentsLeft"] = comments.Count;

            return View();
        }

        [Route("/articles/{*filter}")]
        public IActionResult Articles(string filter)
        {
            if (string.IsNullOrEmpty(filter)) filter = "all";

            List<Article> articles;
            switch (filter)
            {
                case "submitted":
                    ViewData["Message"] = "Article Submissions";
                    articles = _context.Article.Where(x =>
                                   x.Approved == ApprovalStatus.Submitted).ToList();
                    break;
                case "all":
                    ViewData["Message"] = "All Articles";
                    articles = _context.Article.ToList();
                    break;
                default:
                    ViewData["Message"] = "Articles - " + filter;
                    articles = (from a in _context.Article
                                where a.OwnerID == filter ||
                                      a.Content.Contains(filter) ||
                                      a.AuthorName.Contains(filter)
                                select a).ToList();
                    break;
            }

            return View(articles);
        }

        [Route("/comments/{*filter}")]
        public IActionResult Comments(string filter)
        {
            if (string.IsNullOrEmpty(filter)) filter = "all";

            List<Comment> comments;
            switch (filter)
            {
                case "submitted":
                    ViewData["Message"] = "Comment Submissions";
                    comments = _context.Comment.Where(x =>
                                   x.Approved == ApprovalStatus.Submitted).ToList();
                    break;
                case "all":
                    ViewData["Message"] = "All Comments";
                    comments = _context.Comment.ToList();
                    break;
                default:
                    ViewData["Message"] = "Comments - " + filter;
                    comments = (from c in _context.Comment
                                where c.OwnerID == filter ||
                                      c.Content.Contains(filter) ||
                                      c.AuthorName.Contains(filter)
                                select c).ToList();
                    break;
            }

            return View(comments);
        }

        [Route("/users/{filter}/{*sortBy}")]
        public IActionResult Users(string filter, string sortBy = "name")
        {
            if (string.IsNullOrEmpty(filter)) filter = "all";
            ViewData["Filter"] = filter;

            List<OpenTalonUser> users;
            switch (filter)
            {
                case "staff":
                    ViewData["Message"] = "Staff Members";
                    users = _userManager.GetUsersInRoleAsync("Administrator").Result.ToList();
                    break;
                case "all":
                    ViewData["Message"] = "Users";
                    users = _context.Users.ToList();
                    break;
                default:
                    ViewData["Message"] = "Users - " + filter;
                    users = (from u in _context.Users
                             where u.UserName.Contains(filter) || 
                                   u.Email.Contains(filter)
                             select u).ToList();
                    users.AddRange(_userManager.GetUsersInRoleAsync(filter).Result.ToList());
                    break;
            }
            switch (sortBy)
            {
                case "activity":
                    ViewData["SortBy"] = "Activity";
                    users = users.OrderBy(u => u.Articles).ToList();
                    break;
                case "date":
                    ViewData["SortBy"] = "Date";
                    users = users.OrderBy(u => u.DateJoined).ToList();
                    break;
                case "email":
                    ViewData["SortBy"] = "Email";
                    users = users.OrderBy(u => u.Email).ToList();
                    break;
                default:
                    ViewData["SortBy"] = "Name";
                    users = users.OrderBy(u => u.Name).ToList();
                    break;
            }

            Dictionary<OpenTalonUser, List<string>> userRoles = new Dictionary<OpenTalonUser, List<string>>();
            foreach (OpenTalonUser user in users)
                userRoles.Add(user, _userManager.GetRolesAsync(user).Result.ToList());

            return View(userRoles);
        }

        [Route("/tickets/{*filter}")]
        public IActionResult Tickets(string filter)
        {
            List<Ticket> tickets;
            switch (filter)
            {
                case "submitted":
                    ViewData["Message"] = "Submitted Tickets";
                    tickets = _context.Ticket.Where(x =>
                                   x.Approved == HandledStatus.Submitted).ToList();
                    break;
                case "all":
                    ViewData["Message"] = "All Tickets";
                    tickets = _context.Ticket.ToList();
                    break;
                default:
                    ViewData["Message"] = "Tickets - " + filter;
                    tickets = (from t in _context.Ticket
                               where t.OwnerID == filter ||
                                     t.Content.Contains(filter) ||
                                     t.AuthorName.Contains(filter) ||
                                     t.AssignedTo.Contains(filter)
                               select t).ToList();
                    break;
            }

            return View(tickets);
        }

        [Route("/roles")]
        public IActionResult Roles()
        {
            return View();
        }
    }
}