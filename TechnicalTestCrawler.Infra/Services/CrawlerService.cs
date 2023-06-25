using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using TechnicalTestCrawler.Domain.Dto;
using TechnicalTestCrawler.Domain.Services;
using TechnicalTestCrawler.Infra.Context;
using TechnicalTestCrawler.Infra.Entities;

namespace TechnicalTestCrawler.Infra.Services
{
    public class CrawlerService : ICrawlerService
    {
        private const string KabumUrl = "https://www.kabum.com.br/busca";
        private const string FilledStarIconPathStart = "M17";

        private readonly HttpClient _httpClient;
        private readonly DataContext _context;

        public CrawlerService(HttpClient httpClient, DataContext context)
        {
            _httpClient = httpClient;
            _context = context;

            _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
        }

        public async Task CrawlAndStoreData(string searchTerm, int pageNumber = 1)
        {

            var products = await CrawlSite(searchTerm, pageNumber);

            // Armazenar os produtos no banco de dados
            //_context.Products.AddRange(products);
            //await _dbContext.SaveChangesAsync();
        }


        private async Task<List<Product>> CrawlSite(string searchTerm, int pageNumber)
        {
            try
            {
                var products = new List<Product>();

                string html = await _httpClient.GetStringAsync($"{KabumUrl}/{searchTerm}?page_number={pageNumber}");

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var productNodes = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'product')]");

                if (productNodes == null)
                    throw new Exception("Not Found");

                foreach (var productNode in productNodes)
                {
                    var linkElement = productNode.SelectSingleNode(".//a[contains(@class, 'productLink')]");
                    var titleElement = productNode.SelectSingleNode(".//span[contains(@class, 'nameCard')]");
                    var priceElement = productNode.SelectSingleNode(".//span[contains(@class, 'priceCard')]");
                    var imageElement = productNode.SelectSingleNode(".//img[contains(@class, 'imageCard')]");

                    var product = new Product()
                    {
                        Name = titleElement.InnerText,
                        Link = linkElement.Attributes["href"].Value,
                        Price = priceElement.InnerText,
                        ImageUrl = imageElement.Attributes["src"].Value,
                        RefId = linkElement.Attributes["data-smarthintproductid"].Value,
                        Rating = GetRating(productNode)
                    };

                    products.Add(product);
                }


                return products;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private static int GetRating(HtmlNode productNode)
        {
            var ratingCardElement = productNode.SelectSingleNode(".//div[contains(@class, 'ratingCard')]");
            var filledStarCount = -1;
            if (ratingCardElement != null)
            {
                var starSvgPaths = ratingCardElement.SelectNodes(".//div[contains(@class, 'estrelasAvaliacao')]//svg//path");
                filledStarCount = starSvgPaths.Count(x => x.Attributes["d"].Value.StartsWith(FilledStarIconPathStart));
            }

            return filledStarCount;
        }
    }
}
