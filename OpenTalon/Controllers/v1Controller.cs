using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OpenTalon.Data;
using OpenTalon.Models;

namespace OpenTalon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class v1Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public v1Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsLoggedIn()
        {
            if (Request.Headers["AntiForge"] == "UUDDLRLRBABAS") return true;
            if (Request.Cookies["AntiForge"] == "UUDDLRLRBABAS") return true;
            else return false;
        }

        [Route("comment/approve/{*id}")]
        [HttpPost]
        public async Task<string> CommentApprove(int? id)
        {
            if (!IsLoggedIn()) return "403 Forbidden"; 
            if (id == null) return "Requested resource not found.";

            Comment comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                comment.Approved = true;
                await _context.SaveChangesAsync();
            }
            else return $"Comment {id} does not exist in the database.";

            return $"Approved comment {id} successfully!";
        }

        [Route("comment/delist/{*id}")]
        [HttpPost]
        public async Task<string> CommentDelist(int? id)
        {
            if (!IsLoggedIn()) return "403 Forbidden";
            if (id == null) return "Requested resource not found.";

            Comment comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                comment.Approved = false;
                await _context.SaveChangesAsync();
            }
            else return $"Comment {id} does not exist in the database.";

            return $"Delisted comment {id} successfully!";
        }

        [Route("comment/delete/{*id}")]
        [HttpPost]
        public async Task<string> CommentDelete(int? id)
        {
            if (!IsLoggedIn()) return "403 Forbidden";
            if (id == null) return "Requested resource not found.";

            Comment comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
            }
            else return $"Comment {id} does not exist in the database.";

            return $"Deleted comment {id} successfully!";
        }

        [Route("article/approve/{*id}")]
        [HttpPost]
        public async Task<string> ArticleApprove(int? id)
        {
            if (!IsLoggedIn()) return "403 Forbidden";
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                article.Approved = true;
                await _context.SaveChangesAsync();
            }
            else return $"Article {id} does not exist in the database.";

            return $"Approved article {id} successfully!";
        }

        [Route("article/delist/{*id}")]
        [HttpPost]
        public async Task<string> ArticleDelist(int? id)
        {
            if (!IsLoggedIn()) return "403 Forbidden";
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                article.Approved = false;
                await _context.SaveChangesAsync();
            }
            else return $"Article {id} does not exist in the database.";

            return $"Delisted article {id} successfully!";
        }

        [Route("article/delete/{*id}")]
        [HttpPost]
        public async Task<string> ArticleDelete(int? id)
        {
            if (!IsLoggedIn()) return "403 Forbidden";
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            List<Comment> comments = (from c in _context.Comment
                                      where c.PostId == id
                                      select c).ToList();
            if (article != null)
            {
                _context.Article.Remove(article);
                _context.Comment.RemoveRange(comments);
                await _context.SaveChangesAsync();
            }
            else return $"Article {id} does not exist in the database.";

            return $"Deleted article {id} successfully!";
        }

        [Route("article/info/{*id}")]
        [HttpGet]
        public async Task<string> ArticleInfo(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            if (article == null) return $"Article {id} does not exist in the database.";
            if (article.Approved == false && !IsLoggedIn()) return "401 Unauthorized";

            Dictionary<string, string> info = new Dictionary<string, string>()
            {
                { "id", article.Id.ToString() },
                { "title", article.Title },
                { "author", article.AuthorName },
                { "date", article.Date.ToString() },
                { "tags", article.SummaryShort },
                { "summary", article.SummaryLong },
                { "image", article.ThumbnailURL }
            };

            return JsonConvert.SerializeObject(info);
        }

        [Route("article/list/{query}/{*skip}")]
        [HttpGet]
        public string ArticleGetBulk(string query, int skip)
        {
            if (string.IsNullOrEmpty(query)) return "";

            var articles = query.ToLower() switch
            {
                "new" => (from a in _context.Article
                          where a.Approved == true
                          orderby a.Date descending
                          select a)
                          .Skip(skip * 4).Take(4).ToList(),
                _ => (from a in _context.Article
                      where a.Approved == true &&
                      a.Title.Contains(query) || a.SummaryShort.Contains(query)
                      orderby a.Date descending
                      select a)
                      .Skip(skip * 4).Take(4).ToList()
            };

            if (articles.Count == 0) return "";

            List<Dictionary<string, string>> info = new List<Dictionary<string, string>>();
            foreach (Article a in articles)
            {
                Dictionary<string, string> i = new Dictionary<string, string>()
                {
                    { "id", a.Id.ToString() },
                    { "title", a.Title },
                    { "author", a.AuthorName },
                    { "date", a.Date.ToString() },
                    { "tags", a.SummaryShort },
                    { "summary", a.SummaryLong },
                    { "image", a.ThumbnailURL }
                };
                info.Add(i);
            }

            return JsonConvert.SerializeObject(info);
        }

        [Route("generate")]
        public async Task<string> Generate()
        {
            if (!IsLoggedIn()) return "403 Forbidden";
            Article placeholder = new Article
            {
                Approved = true,
                AuthorName = "Kayla!",
                AuthorId = 100023440,
                Date = DateTime.Now,
                Title = "Autogen",
                SummaryShort = "Arts, Opinion, Autogen",
                SummaryLong = "Ever wonder what an autogenerated article looks like?",
                Content = "Now you know!",
                Views = 0
            };

            string[] urls = new string[3] { "https://media.giphy.com/media/l3fQ9icfExRfiePrq/source.gif",
                                            "https://media.giphy.com/media/MJ6SslGZEYKhG/giphy.gif",
                                            "https://static1.e621.net/data/e2/cb/e2cb8f4ffe0e05ddcc8ae93adef08a0e.gif" };
            Random rnd = new Random();
            placeholder.ThumbnailURL = urls[rnd.Next(0, 3)];
            _context.Article.Add(placeholder);
            await _context.SaveChangesAsync();
            return "201 Created";
        }

        [Route("degenerate")]
        public async Task<string> Degenerate()
        {
            if (!IsLoggedIn()) return "403 Forbidden";
            List<Article> a = (from c in _context.Article
                               where c.Title == "Autogen"
                               select c).ToList();
            _context.Article.RemoveRange(a);
            await _context.SaveChangesAsync();
            return "205 Reset";
        }
    } 
}