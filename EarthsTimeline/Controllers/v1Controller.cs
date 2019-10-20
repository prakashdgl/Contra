using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EarthsTimeline.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EarthsTimeline.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class v1Controller : ControllerBase
    {
        private readonly TimelineContext _context;

        public v1Controller(TimelineContext context)
        {
            _context = context;
        }

        [Route("comment/approve/{*id}")]
        [HttpPost]
        public async Task<string> CommentApprove(int? id)
        {
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
            if (id == null) return "";

            Article article = await _context.Article.FindAsync(id);
            if (article == null) return "";

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
    } 
}