using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalCrawler.Domain.Dto;

namespace TechnicalCrawler.Domain.Services
{
    public interface ISearchService
    {
        Task<List<ProductDto>> GetInformation(string searchTerm);
        Task<List<CommentDto>> GetComments(string id);
    }
}
