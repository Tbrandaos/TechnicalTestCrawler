using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PuppeteerSharp;
using TechnicalCrawler.Domain.Dto;
using TechnicalCrawler.Infra.Context;
using TechnicalCrawler.Infra.Entities;
using Product = TechnicalCrawler.Infra.Entities.Product;

namespace TechnicalCrawler.Infra.Services
{
    public class CrawlerService
    {
        private const string KabumUrl = "https://www.kabum.com.br/produto";
        private const string KabumBuscaUrl = "https://www.kabum.com.br/busca";

        private const string FilledStarIconPathStart = "M17";

        private readonly HttpClient _httpClient;

        public CrawlerService()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36");
        }

        public async Task<List<Product>> GetProducts(string searchTerm, int pageNumber = 1)
        {
            try
            {
                var products = new List<Product>();

                string html = await _httpClient.GetStringAsync($"{KabumBuscaUrl}/{searchTerm}?page_number={pageNumber}");

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
                        Link = $"{KabumUrl}/{linkElement.Attributes["data-smarthintproductid"].Value}",
                        Price = priceElement.InnerText,
                        ImageUrl = imageElement.Attributes["src"].Value,
                        RefId = Convert.ToInt32(linkElement.Attributes["data-smarthintproductid"].Value),
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

        public async Task<List<CommentDto>> GetCommentsByProduct(string id)
        {
            var result = new List<CommentDto>();
            var productUrl = $"{KabumUrl}/{id}";

            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);

            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true
            });

            var page = await browser.NewPageAsync();
            await page.GoToAsync(productUrl);

            var hasComments = await LoadComments(page, 3);

            if (!hasComments) return result;
            var commentDivs = await page.QuerySelectorAllAsync("#reviewsSection > div > div");

            // Starts at 1 because the first div is not a comment
            var firstFiveComments = commentDivs.Take(1..6);
            var regexDate = new Regex("\\d{1,2}\\/\\d{1,2}\\/\\d{2,4}");
            foreach (var commentDiv in firstFiveComments)
            {
                var paragraphs = await commentDiv.QuerySelectorAllAsync("p");
                var titleWithDateHandle = await paragraphs[0].GetPropertyAsync("innerText");
                var commentHandle = await paragraphs[2].GetPropertyAsync("innerText");
                var prosHandle = await paragraphs[3].GetPropertyAsync("innerText");
                var consHandle = await paragraphs[4].GetPropertyAsync("innerText");

                var titleWithDate = await titleWithDateHandle.JsonValueAsync<string>();
                var match = regexDate.Match(titleWithDate);

                var date = match.Value;
                var title = titleWithDate.Substring(0, match.Index - 1);
                var comment = await commentHandle.JsonValueAsync<string>();
                var pros = await prosHandle.JsonValueAsync<string>();
                var cons = await consHandle.JsonValueAsync<string>();

                result.Add(new CommentDto()
                {
                    Title = title,
                    Date = date,
                    Comment = comment,
                    Pros = pros,
                    Cons = cons,
                });
            }

            return result;
        }

        private static async Task<bool> LoadComments(IPage page, int times)
        {
            for (int i = 0; i < times; i++)
            {
                var loadMoreButton = await page.QuerySelectorAsync("#reviewsSection > div > button");
                if (loadMoreButton is not null)
                {
                    await loadMoreButton.ClickAsync();
                    await page.WaitForNetworkIdleAsync();
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private static int? GetRating(HtmlNode productNode)
        {
            var ratingCardElement = productNode.SelectSingleNode(".//div[contains(@class, 'ratingCard')]");

            if (ratingCardElement != null)
            {
                var starSvgPaths = ratingCardElement.SelectNodes(".//div[contains(@class, 'estrelasAvaliacao')]//svg//path");
                return starSvgPaths.Count(x => x.Attributes["d"].Value.StartsWith(FilledStarIconPathStart));
            }

            return null;
        }
    }
}
