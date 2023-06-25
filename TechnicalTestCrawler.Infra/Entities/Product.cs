using System.Collections.Generic;

namespace TechnicalTestCrawler.Infra.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public double Rating { get; set; }
        public string RefId { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
