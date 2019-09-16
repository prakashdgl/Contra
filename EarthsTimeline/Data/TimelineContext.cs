using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EarthsTimeline.Models;

namespace EarthsTimeline.Models
{
    public class TimelineContext : DbContext
    {
        public TimelineContext (DbContextOptions<TimelineContext> options)
            : base(options)
        {
        }

        public DbSet<EarthsTimeline.Models.Comment> Comment { get; set; }

        public DbSet<EarthsTimeline.Models.Article> Article { get; set; }
    }
}
