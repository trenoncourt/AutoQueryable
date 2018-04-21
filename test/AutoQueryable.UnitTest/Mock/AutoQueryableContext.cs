using AutoQueryable.UnitTest.Mock.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace AutoQueryable.UnitTest.Mock
{
    public class AutoQueryableContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var consoleLoggerProvider = new ConsoleLoggerProvider((s, level) => true, true);
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(consoleLoggerProvider);
            optionsBuilder.UseLoggerFactory(loggerFactory);
            optionsBuilder.UseInMemoryDatabase("test");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProductExtension>().HasQueryFilter(b => !b.IsDeleted);
        }


        public DbSet<Product> Product { get; set; }
        public DbSet<ProductCategory> ProductCategory { get; set; }
        public DbSet<ProductModel> ProductModel { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetail { get; set; }
    }
}