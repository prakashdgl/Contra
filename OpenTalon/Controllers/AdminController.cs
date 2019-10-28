using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTalon.Data;
using OpenTalon.Models;

namespace OpenTalon.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Route("/admin")]
        public async Task<IActionResult> Index()
        {
            List<Article> articles = await _context.Article.Where((x) => x.Approved != ApprovalStatus.Approved).ToListAsync();
            ViewData["Articles"] = _context.Article.Count();
            ViewData["ArticlesLeft"] = articles.Count;

            List<Comment> comments = await _context.Comment.Where((x) => x.Approved != ApprovalStatus.Approved).ToListAsync();
            ViewData["Comments"] = _context.Comment.Count();
            ViewData["CommentsLeft"] = comments.Count;

            return View();
        }

        [Route("/articles/{*filter}")]
        public async Task<IActionResult> Articles(string filter)
        {
            if (filter == "submitted")
            {
                ViewData["Message"] = "Submissions";
                return View(await _context.Article.Where(x => x.Approved == ApprovalStatus.Submitted).ToListAsync());
            }

            ViewData["Message"] = "All Articles";
            return View(await _context.Article.ToListAsync());
        }

        [Route("/comments/{*filter}")]
        public async Task<IActionResult> Comments(string filter)
        {
            if (filter == "submitted")
            {
                ViewData["Message"] = "Submissions";
                return View(await _context.Comment.Where(x => x.Approved == ApprovalStatus.Submitted).ToListAsync());
            }

            ViewData["Message"] = "All Comments";
            return View(await _context.Comment.ToListAsync());
        }

        [Route("/users")]
        public async Task<IActionResult> Users()
        {
            return View();
        }
    }
}