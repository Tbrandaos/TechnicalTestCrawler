
namespace TechnicalCrawler.Domain.Dto
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public string Price { get; set; }
        public string ImageUrl { get; set; }
        public int? Rating { get; set; }
        public string RefId { get; set; }
    }
}
