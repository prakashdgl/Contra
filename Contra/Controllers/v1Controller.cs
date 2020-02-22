﻿using System;
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
    [Authorize(Roles = "Administrator")]
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

        [HttpPost("role/create/{*role}")]
        public async Task<string> CreateRole(string role)
        {
            if (_roleManager.RoleExistsAsync(role).Result)
                return "Role already exists!";

            await _roleManager.CreateAsync(new IdentityRole(role));

            return $"Created {role} successfully!";
        }

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

        [HttpPost("user/{userID}/ban")]
        public async Task<string> BanUser(string userID)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userID);
            if (user == null)
                return "Requested user not found.";

            user.IsBanned = true;
            await _userManager.UpdateAsync(user);
            
            return $"Banned {user.Name}!";
        }

        [HttpPost("user/{userID}/unban")]
        public async Task<string> UnbanUser(string userID)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userID);
            if (user == null)
                return "Requested user not found.";

            user.IsBanned = false;
            await _userManager.UpdateAsync(user);

            return $"Unbanned {user.Name}!";
        }

        [HttpPost("user/{userID}/remove")]
        public async Task<string> RemoveUser(string userID)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userID);
            if (user == null)
                return "Requested user not found.";

            if (!user.EmailConfirmed)
                await _userManager.DeleteAsync(user);
            else
                return "Cannot remove user!";

            return $"Removed {user.Name}!";
        }

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

        [Authorize(Roles = "Staff, Administrator")]
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

        [Authorize(Roles = "Staff, Administrator")]
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

        [Authorize(Roles = "Staff, Administrator")]
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

        [Authorize(Roles = "Staff, Administrator")]
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

        [Authorize(Roles = "Staff, Administrator")]
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

        [Authorize(Roles = "Staff, Administrator")]
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

        [AllowAnonymous]
        [HttpGet("account/{id}/picture")]
        public async Task<string> GetProfilePicture(string id)
        {
            ContraUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return user.ProfilePictureURL;
            else return "Not found!";
        }

        [AllowAnonymous]
        [HttpGet("article/list/{query}/{amount}/{*skip}")]
        public string ArticleGetBulk(string query, int amount, int skip)
        {
            if (string.IsNullOrEmpty(query)) return "";
            if (query.StartsWith("blog::"))
                return new v2Controller(_context).BlogGetBulk(query.Substring(6), amount, skip);

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
                    "creative" => (from a in _context.Article
                                   where a.Approved == ApprovalStatus.Approved &&
                                         a.IsArchived != true &&
                                         a.ArticleType == ArticleType.Creative
                                   orderby a.IsPinned descending, a.Date descending
                                   select a).ToList(),
                    "editorial" => (from a in _context.Article
                                    where a.Approved == ApprovalStatus.Approved &&
                                          a.IsArchived != true &&
                                          a.ArticleType == ArticleType.Article &&
                                          a.IsEditorial
                                    orderby a.Date descending
                                    select a).ToList(),
                    "new" => (from a in _context.Article
                              where a.Approved == ApprovalStatus.Approved &&
                                    a.IsArchived != true &&
                                    a.ArticleType != ArticleType.Blog
                              orderby a.Date descending
                              select a).ToList(),
                    "insight" => (from a in _context.Article
                                  where a.Approved == ApprovalStatus.Approved &&
                                        a.IsArchived != true &&
                                        a.ArticleType == ArticleType.Insight
                                  orderby a.IsPinned descending, a.Date descending
                                  select a).ToList(),
                    "response" => (from a in _context.Article
                                   where a.Approved == ApprovalStatus.Approved &&
                                         a.IsArchived != true &&
                                        (a.ArticleType == ArticleType.Response ||
                                         a.ArticleType == ArticleType.Meta)
                                   orderby a.IsPinned descending, a.Date descending
                                   select a).ToList(),
                    _ => (from a in _context.Article
                          where a.Approved == ApprovalStatus.Approved &&
                                a.ArticleType != ArticleType.Blog &&
                                a.IsArchived != true &&
                               (a.Title.ToLower().Contains(query) ||
                                a.Tags.ToLower().Contains(query) ||
                                (a.OwnerID == query && !a.Anonymous))
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

        [AllowAnonymous]
        [HttpGet("article/{id}")]
        public string ArticleGet(int id)
        {
            Article a = (from c in _context.Article
                         where c.Id == id &&
                               c.Approved == ApprovalStatus.Approved
                         select c).FirstOrDefault();

            if (a == null) return "";

            Dictionary<string, string> i = new Dictionary<string, string>()
            {
                { "id", a.Id.ToString() },
                { "title", a.Title },
                { "author", a.AuthorName },
                { "date", a.Date.ToString() },
                { "tags", a.Tags },
                { "summary", a.SummaryLong },
                { "content", a.Content },
                { "image", a.ThumbnailURL },
                { "pinned", a.IsPinned.ToString() },
                { "sensitive", a.Sensitive.ToString() },
                { "spoiler", a.Spoiler.ToString() }
            };

            if (a.Anonymous)
                i["author"] = "Anonymous";

            return JsonConvert.SerializeObject(i);
        }

        [Route("generate/{tags}/{type}")]
        public async Task<string> Generate(string tags, int type)
        {
            Article placeholder = new Article
            {
                Approved = ApprovalStatus.Approved,
                AuthorName = (await _userManager.GetUserAsync(User)).Name,
                OwnerID = "Autogen",
                Date = DateTime.Now,
                Title = "Autogen",
                Tags = tags,
                SummaryLong = "Ever wonder what an autogenerated article looks like?",
                Content = "Now you know!",
                Views = 0
            };

            placeholder.ArticleType = (ArticleType)type;

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