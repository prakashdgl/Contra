using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenTalon.Areas.Identity.Data;

namespace OpenTalon.Data
{
    public class ApplicationDbContext : IdentityDbContext<OpenTalonUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Comment> Comment { get; set; }

        public DbSet<Models.Article> Article { get; set; }
    }
}
