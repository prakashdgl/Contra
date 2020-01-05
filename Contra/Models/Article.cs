using System;

namespace Contra.Models
{
    public class Article
    {
        public int Id { get; set; }

        public string OwnerID { get; set; }

        public ApprovalStatus Approved { get; set; }
        public ArticleType ArticleType { get; set; }
        public string AuthorName { get; set; }
        public string ThumbnailURL { get; set; }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Tags { get; set; }
        public string SummaryLong { get; set; }

        public int Views { get; set; }
        public int Likes { get; set; }

        public string Content { get; set; }

        public bool IsEditorial { get; set; }
        public bool IsPinned { get; set; }

        public bool Anonymous { get; set; }
        public bool Sensitive { get; set; }
        public bool Spoiler { get; set; }
    }

    public enum ApprovalStatus
    {
        Submitted, 
        Approved, 
        Rejected
    }

    public enum ArticleType
    {
        Article,
        Event,
        Newsbeat
    }
}
