using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCrawler.Domain.Dto;
using TechnicalCrawler.Domain.Services;
using TechnicalCrawler.Infra.Context;
using TechnicalCrawler.Infra.Entities;

namespace TechnicalCrawler.Infra.Services
{
    public class SearchService : ISearchService
    {
        private readonly DataContext _context;
        private readonly CrawlerService _crawlerService;
        private readonly IMapper _mapper;

        public SearchService(DataContext context, CrawlerService crawlerService, IMapper mapper)
        {
            _context = context;
            _crawlerService = crawlerService;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> GetInformation(string searchTerm)
        {
            try
            {
                var products = await _crawlerService.GetProducts(searchTerm);

                if (GetInput(searchTerm) == null)
                {
                    _context.Products.AddRange(products);
                    _context.InputControl.Add(new InputControl()
                    {
                        SearchDate = DateTime.UtcNow,
                        Searchterm = searchTerm
                    });

                    await _context.SaveChangesAsync();
                }

                return _mapper.Map<List<ProductDto>>(products);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<CommentDto>> GetComments(string id)
        {
            try
            {
                return await _crawlerService.GetCommentsByProduct(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private InputControl GetInput(string searchterm)
        {
            try
            {
                DateTime searchDate = DateTime.UtcNow.AddDays(-2).ToUniversalTime();
                return _context.InputControl.FirstOrDefault(a => a.Searchterm.Contains(searchterm) && a.SearchDate > searchDate);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
