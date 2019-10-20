using System;

namespace EarthsTimeline.Models
{
    public class Article
    {
        public int Id { get; set; }
        public bool Approved { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string ThumbnailURL { get; set; }

        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string SummaryShort { get; set; }
        public string SummaryLong { get; set; }

        public int Views { get; set; }

        public string Content { get; set; }
    }
}
