using System.Collections.Generic;
using System.Threading.Tasks;
using Contra.Data;
using Contra.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
