using Microsoft.AspNetCore.Identity;
using OpenTalon.Models;
using System;
using System.Collections.Generic;

namespace OpenTalon.Areas.Identity.Data
{
    public class OpenTalonUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        public DateTime DateJoined { get; set; }

        public List<Article> Articles { get; set; }
        public List<Article> ArticlesLiked { get; set; }
        public List<Article> ArticlesViewed { get; set; }

        public List<Comment> Comments { get; set; }
        public List<Comment> CommentsLiked { get; set; }
    }
}
