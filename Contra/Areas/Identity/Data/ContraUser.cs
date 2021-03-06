﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Contra.Models;

namespace Contra.Areas.Identity.Data
{
    public class ContraUser : IdentityUser
    {
        [PersonalData]
        public string Name { get; set; }
        [PersonalData]
        public string Bio { get; set; }
        [PersonalData]
        public string ProfilePictureURL { get; set; }
        [PersonalData]
        public DateTime DateJoined { get; set; }
        public bool IsBanned { get; set; }

        public List<Article> Articles { get; set; }
        public List<Article> ArticlesLiked { get; set; }
        public List<Article> ArticlesViewed { get; set; }

        public List<Comment> CommentsLiked { get; set; }
    }
}
