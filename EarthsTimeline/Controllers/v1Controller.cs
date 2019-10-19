using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EarthsTimeline.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [Route("comment/delete/{*id}")]
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
    }
}