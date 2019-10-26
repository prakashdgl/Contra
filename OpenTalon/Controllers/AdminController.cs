using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenTalon.Data;
using OpenTalon.Models;

namespace OpenTalon.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
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
                        new CookieOptions() { Path = "/", Expires = DateTime.Now.AddDays(1), IsEssential = true });
                return Redirect("~/admin");
            }
            else return View();
        }

        [Route("/admin")]
        public async Task<IActionResult> Index()
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
    }
}