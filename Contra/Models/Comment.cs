using System;

namespace Contra.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string OwnerID { get; set; }

        public ApprovalStatus Approved { get; set; }
        public int PostId { get; set; }
        public DateTime Date { get; set; }
        public string AuthorName { get; set; }
        public string Content { get; set; }
    }
}
