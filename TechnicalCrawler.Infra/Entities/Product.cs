using System.Collections.Generic;

namespace TechnicalCrawler.Infra.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public int? Rating { get; set; }
        public int RefId { get; set; }
        //public List<Comment> Comments { get; set; }
    }
}
