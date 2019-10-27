using System;

namespace OpenTalon.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public ApprovalStatus Approved { get; set; }
        public int PostId { get; set; }
        public DateTime Date { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
    }
}
