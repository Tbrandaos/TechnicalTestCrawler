using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using TechnicalTestCrawler.Domain.Services;

namespace TechnicalTestCrawler.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CrawlerController : Controller
    {
        private readonly ISearchService _searchService;

        public CrawlerController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Runs the crawler and stores the data in the database")]
        public async Task<IActionResult> CrawlAndStoreData([FromBody] string searchterm)
        {
            try
            {
                var response = _searchService.GetInformation(searchterm);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error running crawler: {ex.Message}");
            }
        }
    }
}
