using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Contra.Data;
using Contra.Models;
using Contra.Areas.Identity.Data;
using Ganss.XSS;
using System.Linq;

namespace Contra.Controllers
{
    [Authorize]
    public class SubmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ContraUser> _userManager;

        private static readonly List<string> SubmitFullAllowedTypes = new List<string>()
        {
            "article", "insight"
        };
        private static readonly List<string> SubmitQuickAllowedTypes = new List<string>()
        {
            "creative", "meta"
        };

        public SubmissionsController(ApplicationDbContext context,
                                     UserManager<ContraUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet("/submit")]
        public IActionResult Submit()
        {
            return View();
        }

        // Check if image is < 2MB and is either JPEG or PNG
        private bool ValidateImage(IFormFile image)
        {
            if (image == null) return false;

            List<string> allowedTypes = new List<string> { "image/png", "image/jpeg" };
            if (!allowedTypes.Contains(image.ContentType)) return false;

            if (!(image.Length > 0 && image.Length < 2000000)) return false;

            return true;
        }

        // Adds an image to the database, associates it with a user, and returns the URL
        private async Task<string> UploadImage(string ownerID, IFormFile image) {
            string name = Path.GetRandomFileName();

            Image i = new Image
            {
                OwnerID = ownerID,
                Name = name,
                ContentType = image.ContentType,
            };

            using (var rs = image.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                rs.CopyTo(ms);
                i.Content = ms.ToArray();
            }

            _context.Image.Add(i);
            await _context.SaveChangesAsync();

            return "/img/upload/" + name;
        }

        [HttpGet("/submit/full/{*type}")]
        public IActionResult SubmitFull(string type)
        {
            if (!SubmitFullAllowedTypes.Contains(type.ToLower()))
                return Redirect("/submit");
            else
                return View();
        }

        [HttpPost("/submit/full/{*type}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitFull(string type, [Bind("Id,AuthorName,Title,Tags,Content,Anonymous,Sensitive,Spoiler")] Article article, IFormFile thumbnail)
        {
            if (ModelState.IsValid)
            {
                ContraUser user = await _userManager.GetUserAsync(User);
                if (user.IsBanned) return Redirect("/Identity/Account/Login");

                if (ValidateImage(thumbnail))
                    article.ThumbnailURL = await UploadImage(user.Id, thumbnail);
                else
                    return View(article);

                switch (type.ToLower())
                {
                    case "article":
                        article.ArticleType = ArticleType.Article;
                        break;
                    case "insight":
                        article.ArticleType = ArticleType.Insight;
                        break;
                    default:
                        return Redirect("/submit");
                }

                article.OwnerID = user.Id;

                if (article.Anonymous)
                    article.AuthorName = "Anonymous";
                else if (!string.IsNullOrWhiteSpace(article.AuthorName))
                    article.AuthorName = user.Name + ", " + article.AuthorName;
                else
                    article.AuthorName = user.Name;

                article.Date = DateTime.Now;
                article.Views = 0;
                article.Likes = 0;

                if (User.IsInRole("Staff"))
                {
                    article.Approved = ApprovalStatus.Approved;
                    article.IsEditorial = true;
                }
                else
                    article.Approved = ApprovalStatus.Submitted;

                HtmlSanitizer sanitizer = new HtmlSanitizer();
                article.Content = sanitizer.Sanitize(article.Content);

                article.SummaryLong = Regex.Replace(article.Content, @"<[^>]*>", string.Empty).Trim().Substring(0, 60) + "...";

                if (user.Articles == null)
                    user.Articles = new List<Article>();
                user.Articles.Add(article);

                _context.Article.Add(article);
                await _context.SaveChangesAsync();
                return Redirect("~/success");
            }

            return View(article);
        }

        [HttpGet("/submit/quick/{*type}")]
        public IActionResult SubmitQuick(string type)
        {
            if (!SubmitQuickAllowedTypes.Contains(type.ToLower()))
                return Redirect("/submit");
            else
                return View();
        }

        [HttpPost("/submit/quick/{*type}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitQuick(string type, [Bind("Id,AuthorName,Title,Content,Anonymous,Sensitive,Spoiler")] Article article, IFormFile thumbnail)
        {
            if (ModelState.IsValid)
            {
                ContraUser user = await _userManager.GetUserAsync(User);
                if (user.IsBanned) return Redirect("/Identity/Account/Login");

                if (ValidateImage(thumbnail))
                    article.ThumbnailURL = await UploadImage(user.Id, thumbnail);
                else
                    return View(article);

                switch (type.ToLower())
                {
                    case "creative":
                        article.ArticleType = ArticleType.Creative;
                        break;
                    case "meta":
                        article.ArticleType = ArticleType.Meta;
                        break;
                    default:
                        return Redirect("/submit");
                }

                article.OwnerID = user.Id;

                if (article.Anonymous)
                    article.AuthorName = "Anonymous";
                else if (!string.IsNullOrWhiteSpace(article.AuthorName))
                    article.AuthorName = user.Name + ", " + article.AuthorName;
                else
                    article.AuthorName = user.Name;

                article.Date = DateTime.Now;
                article.Views = 0;
                article.Likes = 0;

                if (User.IsInRole("Staff"))
                {
                    article.Approved = ApprovalStatus.Approved;
                    article.IsEditorial = true;
                }
                else
                    article.Approved = ApprovalStatus.Submitted;

                HtmlSanitizer sanitizer = new HtmlSanitizer();
                article.Content = sanitizer.Sanitize(article.Content);

                article.SummaryLong = Regex.Replace(article.Content, @"<[^>]*>", string.Empty).Trim().Substring(0, 60) + "...";

                if (user.Articles == null)
                    user.Articles = new List<Article>();
                user.Articles.Add(article);

                _context.Article.Add(article);
                await _context.SaveChangesAsync();
                return Redirect("~/success");
            }

            return View(article);
        }

        [HttpGet("/submit/response/{*responseId}")]
        public IActionResult SubmitResponse(int? responseId = null)
        {
            if (responseId.HasValue)
                ViewData["ResponseId"] = $"https://contra.live/article/{responseId}";
            return View();
        }

        [HttpPost("/submit/response/{*responseId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitResponse([Bind("Id,ResponseId,AuthorName,Title,Content,Anonymous,Sensitive,Spoiler")] Article article, IFormFile thumbnail)
        {
            if (ModelState.IsValid)
            {
                ContraUser user = await _userManager.GetUserAsync(User);
                if (user.IsBanned) return Redirect("/Identity/Account/Login");

                if (ValidateImage(thumbnail))
                    article.ThumbnailURL = await UploadImage(user.Id, thumbnail);
                else
                    return View(article);

                article.ArticleType = ArticleType.Response;
                if (!_context.Article.Any(a => a.Id == article.ResponseId))
                    return View(article);

                article.OwnerID = user.Id;

                if (article.Anonymous)
                    article.AuthorName = "Anonymous";
                else if (!string.IsNullOrWhiteSpace(article.AuthorName))
                    article.AuthorName = user.Name + ", " + article.AuthorName;
                else
                    article.AuthorName = user.Name;

                article.Date = DateTime.Now;
                article.Views = 0;
                article.Likes = 0;

                if (User.IsInRole("Staff"))
                {
                    article.Approved = ApprovalStatus.Approved;
                    article.IsEditorial = true;
                }
                else
                    article.Approved = ApprovalStatus.Submitted;

                HtmlSanitizer sanitizer = new HtmlSanitizer();
                article.Content = sanitizer.Sanitize(article.Content);

                article.SummaryLong = Regex.Replace(article.Content, @"<[^>]*>", string.Empty).Trim().Substring(0, 60) + "...";

                _context.Article.Add(article);
                await _context.SaveChangesAsync();
                return Redirect("~/success");
            }

            return View(article);
        }

    }
}
