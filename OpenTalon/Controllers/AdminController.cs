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
        public async Task<IActionResult> Articles(string filter)
        {
            if (filter == "submitted")
            {
                ViewData["Message"] = "Article Submissions";
                return View(await _context.Article.Where(x => 
                            x.Approved == ApprovalStatus.Submitted).ToListAsync());
            }

            ViewData["Message"] = "All Articles";
            return View(await _context.Article.ToListAsync());
        }

        [Route("/comments/{*filter}")]
        public async Task<IActionResult> Comments(string filter)
        {
            if (filter == "submitted")
            {
                ViewData["Message"] = "Comment Submissions";
                return View(await _context.Comment.Where(x => 
                            x.Approved == ApprovalStatus.Submitted).ToListAsync());
            }

            ViewData["Message"] = "All Comments";
            return View(await _context.Comment.ToListAsync());
        }

        [Route("/users/{*filter}")]
        public async Task<IActionResult> Users(string filter)
        {
            if (filter == "staff")
            {
                ViewData["Message"] = "Staff Members";
                return View(_userManager.GetUsersInRoleAsync("Administrator").Result);
            }

            ViewData["Message"] = "Users";
            return View(await _context.Users.ToListAsync());
        }
    }
}