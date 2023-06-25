using System.Threading.Tasks;

namespace TechnicalTestCrawler.Domain.Services
{
    public interface ICrawlerService
    {
        Task CrawlAndStoreData(string searchTerm, int pageNumber = 1);
    }
}
