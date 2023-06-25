using System;

namespace TechnicalCrawler.Infra.Entities
{
    public class InputControl
    {
        public int Id { get; set; }
        public string Searchterm { get; set; }
        public DateTime SearchDate { get; set; }
    }
}
