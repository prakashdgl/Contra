using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Contra.Areas.Identity.Data;
using Contra.Data;
using Contra.Models;

namespace Contra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class v1Controller : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ContraUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public v1Controller(ApplicationDbContext context,
                            UserManager<ContraUser> userManager,
                            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("role/create/{*role}")]
        public async Task<string> CreateRole(string role)
        {
            if (_roleManager.RoleExistsAsync(role).Result)
                return "Role already exists!";

            await _roleManager.CreateAsync(new IdentityRole(role));

            return $"Created {role} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("role/delete/{*role}")]
        public async Task<string> DeleteRole(string role)
        {
            if (!_roleManager.RoleExistsAsync(role).Result)
                return "Role does not exist!";
            if (role == "Administrator" || role == "Staff") 
                return "Role cannot be deleted!";

            await _roleManager.DeleteAsync(_roleManager.FindByNameAsync(role).Result);

            return $"Deleted {role} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("user/{userID}/ensure/{*role}")]
        public async Task<string> EnsureRole(string userID, string role)
        {
            if (!_roleManager.RoleExistsAsync(role).Result)
                return "Requested role not found.";

            var user = _userManager.Users.FirstOrDefault(u => u.Id == userID);
            if (user == null)
                return "Requested user not found.";

            await _userManager.AddToRoleAsync(user, role);
            return $"Added {role} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("user/{userID}/enfeeble/{*role}")]
        public async Task<string> EnfeebleRole(string userID, string role)
        {
            if (!_roleManager.RoleExistsAsync(role).Result)
                return "Requested role not found.";

            var user = _userManager.Users.FirstOrDefault(u => u.Id == userID);
            if (user == null)
                return "Requested user not found.";

            await _userManager.RemoveFromRoleAsync(user, role);
            return $"Removed {role} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("comment/approve/{*id}")]
        public async Task<string> CommentApprove(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Comment comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                comment.Approved = ApprovalStatus.Approved;
                await _context.SaveChangesAsync();
            }
            else return $"Comment {id} does not exist in the database.";

            return $"Approved comment {id} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("comment/delist/{*id}")]
        public async Task<string> CommentDelist(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Comment comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                comment.Approved = ApprovalStatus.Rejected;
                await _context.SaveChangesAsync();
            }
            else return $"Comment {id} does not exist in the database.";

            return $"Delisted comment {id} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("comment/delete/{*id}")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPost("article/approve/{*id}")]
        public async Task<string> ArticleApprove(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                article.Approved = ApprovalStatus.Approved;
                await _context.SaveChangesAsync();
            }
            else return $"Article {id} does not exist in the database.";

            return $"Approved article {id} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("article/delist/{*id}")]
        public async Task<string> ArticleDelist(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                article.Approved = ApprovalStatus.Rejected;
                await _context.SaveChangesAsync();
            }
            else return $"Article {id} does not exist in the database.";

            return $"Delisted article {id} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("article/delete/{*id}")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPost("article/pin/{*id}")]
        public async Task<string> ArticlePin(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                article.IsPinned = true;
                await _context.SaveChangesAsync();
            }
            else return $"Article {id} does not exist in the database.";

            return $"Pinned article {id} successfully!";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("article/unpin/{*id}")]
        public async Task<string> ArticleUnpin(int? id)
        {
            if (id == null) return "Requested resource not found.";

            Article article = await _context.Article.FindAsync(id);
            if (article != null)
            {
                article.IsPinned = false;
                await _context.SaveChangesAsync();
            }
            else return $"Article {id} does not exist in the database.";

            return $"Unpinned article {id} successfully!";
        }

        [Authorize]
        [HttpPost("account/{id}/picture/{*url}")]
        public async Task<string> ChangeProfilePicture(string id, string url)
        {
            if (_userManager.GetUserId(User) == id)
            {
                ContraUser user = await _userManager.GetUserAsync(User);
                if (url == "reset")
                {
                    StringBuilder sb = new StringBuilder();
                    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                    {
                        byte[] inputBytes = Encoding.ASCII.GetBytes(user.Email.Trim().ToLower());
                        byte[] hashBytes = md5.ComputeHash(inputBytes);

                        for (int i = 0; i < hashBytes.Length; i++)
                            sb.Append(hashBytes[i].ToString("X2"));
                    }
                    user.ProfilePictureURL = "https://gravatar.com/avatar/" + sb.ToString() + "?d=identicon";
                    await _userManager.UpdateAsync(user);
                }
                else
                {
                    user.ProfilePictureURL = url;
                    await _userManager.UpdateAsync(user);
                }

                return "Successfully changed profile picture!";
            }
            else return "Not authorized!";
        }

        [HttpGet("account/{id}/picture")]
        public async Task<string> GetProfilePicture(string id)
        {
            ContraUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return user.ProfilePictureURL;
            else return "Not found!";
        }

        [HttpGet("article/list/{query}/{amount}/{*skip}")]
        public string ArticleGetBulk(string query, int amount, int skip)
        {
            if (string.IsNullOrEmpty(query)) return "";

            List<Article> articles;
            if (_userManager.GetUserId(User) == query)
            {
                articles = (from a in _context.Article
                            where a.OwnerID == query
                            orderby a.Date descending
                            select a).ToList();
            }
            else
            {
                articles = (query.ToLower()) switch
                {
                    "editorial" => (from a in _context.Article
                                    where a.Approved == ApprovalStatus.Approved &&
                                          a.ArticleType == ArticleType.Article &&
                                          a.IsEditorial
                                    orderby a.Date descending
                                    select a).ToList(),
                    "event" => (from a in _context.Article
                                   where a.Approved == ApprovalStatus.Approved &&
                                         a.ArticleType == ArticleType.Event
                                   orderby a.IsPinned descending, a.Date descending
                                   select a).ToList(),
                    "new" => (from a in _context.Article
                              where a.Approved == ApprovalStatus.Approved &&
                                    a.ArticleType == ArticleType.Article
                              orderby a.Date descending
                              select a).ToList(),
                    "newsbeat" => (from a in _context.Article
                                   where a.Approved == ApprovalStatus.Approved &&
                                         a.ArticleType == ArticleType.Newsbeat
                                   orderby a.IsPinned descending, a.Date descending
                                   select a).ToList(),
                    _ => (from a in _context.Article
                          where a.Approved == ApprovalStatus.Approved &&
                               (a.Title.ToLower().Contains(query) ||
                                a.Tags.ToLower().Contains(query) ||
                                a.OwnerID == query)
                          orderby a.Date descending
                          select a).ToList()
                };
            }

            articles = articles.Skip(skip * amount).Take(amount).ToList();
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

        [Authorize(Roles = "Administrator")]
        [Route("generate/{tags}/{isNewsbeat}/{isEditorial}")]
        public async Task<string> Generate(string tags, bool isNewsbeat = false, bool isEditorial = false)
        {
            Article placeholder = new Article
            {
                ArticleType = ArticleType.Article,
                Approved = ApprovalStatus.Approved,
                AuthorName = "Sei",
                OwnerID = "Autogen",
                Date = DateTime.Now,
                Title = "Autogen",
                Tags = tags,
                SummaryLong = "Ever wonder what an autogenerated article looks like?",
                Content = "Now you know!",
                Views = 0
            };

            if (isNewsbeat) placeholder.ArticleType = ArticleType.Newsbeat;
            if (isEditorial) placeholder.IsEditorial = true;

            string[] urls = new string[5] { "/img/img01.jpg",
                                            "/img/img02.jpg",
                                            "/img/img03.jpg",
                                            "/img/img04.jpg",
                                            "/img/img05.jpg" };
            Random rnd = new Random();
            placeholder.ThumbnailURL = urls[rnd.Next(0, 5)];
            _context.Article.Add(placeholder);
            await _context.SaveChangesAsync();
            return "Created Article with ID: " + placeholder.Id;
        }

        [Authorize(Roles = "Administrator")]
        [Route("degenerate")]
        public async Task<string> Degenerate()
        {
            List<Article> a = (from c in _context.Article
                               where c.OwnerID == "Autogen"
                               select c).ToList();
            _context.Article.RemoveRange(a);
            await _context.SaveChangesAsync();
            return "Reset";
        }
    } 
}