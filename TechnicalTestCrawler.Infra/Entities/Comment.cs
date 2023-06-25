
using System;

namespace TechnicalTestCrawler.Infra.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
    }
}
