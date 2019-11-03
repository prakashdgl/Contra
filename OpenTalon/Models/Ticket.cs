using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTalon.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        public string OwnerID { get; set; }
        public string AssignedTo { get; set; }

        public HandledStatus Approved { get; set; }
        public string AuthorName { get; set; }

        public string Title { get; set; }
        public string Tags { get; set; }
        public DateTime Date { get; set; }

        public string Content { get; set; }
    }

    public enum HandledStatus
    {
        Submitted,
        Reviewed,
        Assigned,
        Active,
        Handled
    }
}
