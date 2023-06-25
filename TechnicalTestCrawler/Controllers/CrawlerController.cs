using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using TechnicalTestCrawler.Domain.Services;

namespace TechnicalTestCrawler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlerController : ControllerBase
    {
        private readonly ICrawlerService _crawlerService;

        public CrawlerController(ICrawlerService crawlerService)
        {
            _crawlerService = crawlerService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Runs the crawler and stores the data in the database")]
        public async Task<IActionResult> CrawlAndStoreData([FromBody] string searchterm)
        {
            try
            {
                await _crawlerService.CrawlAndStoreData(searchterm);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error running crawler: {ex.Message}");
            }
        }
    }
}
