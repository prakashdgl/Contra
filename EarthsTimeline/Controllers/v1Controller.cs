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
        public string CommentApprove(int id)
        {
            Comment comment = _context.Comment.Where(x => x.Id == id).FirstOrDefault();
            if (comment != null)
            {
                comment.Approved = true;
                _context.SaveChanges();
            }
            else return $"Comment {id} does not exist in the database.";

            return $"Approved comment {id} successfully!";
        }
    }
}