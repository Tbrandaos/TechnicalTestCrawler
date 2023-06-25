using Microsoft.EntityFrameworkCore;
using TechnicalCrawler.Infra.Entities;

namespace TechnicalCrawler.Infra.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<InputControl> InputControl { get; set; }
        //public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
