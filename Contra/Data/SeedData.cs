using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Contra.Areas.Identity.Data;
using Contra.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Contra.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            // Use this password to login and set up perms, and change this password.
            string testUserPw = "Testing1!";

            var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@contra.live");
            await EnsureRole(serviceProvider, adminID, "Administrator");
            await EnsureRole(serviceProvider, adminID, "Staff");
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<ContraUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                // Create MD5 Hash for Gravatar
                StringBuilder sb = new StringBuilder();
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(UserName);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    for (int i = 0; i < hashBytes.Length; i++)
                        sb.Append(hashBytes[i].ToString("X2"));
                }

                user = new ContraUser
                {
                    Name = "Qi",

                    Articles = new List<Article>(),
                    ArticlesLiked = new List<Article>(),
                    ArticlesViewed = new List<Article>(),
                    CommentsLiked = new List<Comment>(),

                    ProfilePictureURL = "https://gravatar.com/avatar/" + sb.ToString() + "?d=identicon",
                    UserName = UserName,
                    Email = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }
            if (user == null) throw new Exception("The password is probably not strong enough!");

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (roleManager == null) throw new Exception("roleManager null");

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
                _ = await roleManager.CreateAsync(new IdentityRole(role));

            var userManager = serviceProvider.GetService<UserManager<ContraUser>>();
            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
                throw new Exception("The testUserPw password was probably not strong enough!");

            IR = await userManager.AddToRoleAsync(user, role);
            return IR;
        }
    }
}
