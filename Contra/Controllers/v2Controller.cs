using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contra.Data;
using Contra.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Contra.Controllers
{
    [Authorize(Roles = "Administrator")]
    [Route("api/[controller]")]
    [ApiController]
    public class v2Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public v2Controller(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("ticket/delete/{*id}")]
        public async Task<string> TicketDelete(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Ticket ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null)
            {
                _context.Ticket.Remove(ticket);
                await _context.SaveChangesAsync();
            }
            else return $"Ticket {id} does not exist in the database.";

            return $"Deleted ticket {id} successfully!";
        }

        [AllowAnonymous]
        [HttpGet("blog/list/{id}/{amount}/{*skip}")]
        public string BlogGetBulk(string id, int amount, int skip)
        {
            if (string.IsNullOrEmpty(id)) 
                return "";

            List<Article> blogs;
            blogs = (from a in _context.Article
                     where a.Approved == ApprovalStatus.Approved &&
                           a.ArticleType == ArticleType.Blog &&
                           a.OwnerID == id
                     orderby a.Date descending
                     select a).ToList();

            blogs = blogs.Skip(skip * amount).Take(amount).ToList();
            if (blogs.Count == 0) return "";

            List<Dictionary<string, string>> info = new List<Dictionary<string, string>>();
            foreach (Article a in blogs)
            {
                Dictionary<string, string> i = new Dictionary<string, string>()
                {
                    { "id", a.Id.ToString() },
                    { "title", a.Title },
                    { "author", a.AuthorName },
                    { "date", a.Date.ToString() },
                    { "tags", a.Tags },
                    { "summary", a.SummaryLong },
                    { "image", a.ThumbnailURL },
                    { "pinned", a.IsPinned.ToString() },
                    { "sensitive", a.Sensitive.ToString() },
                    { "spoiler", a.Spoiler.ToString() }
                };

                if (a.Anonymous)
                    i["author"] = "Anonymous";

                info.Add(i);
            }

            return JsonConvert.SerializeObject(info);
        }

        [HttpPost("article/archive/all")]
        public string ArchiveAll()
        {
            foreach(Article a in _context.Article)
            {
                a.IsArchived = true;
                _context.Article.Update(a);
            }

            return "Archived all.";
        }
    }
}
