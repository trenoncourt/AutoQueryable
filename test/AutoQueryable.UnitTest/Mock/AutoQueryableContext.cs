using System.Reflection.Metadata.Ecma335;
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
            optionsBuilder.UseInMemoryDatabase();
        }
        
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductModel> ProductModel { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetail { get; set; }
    }
}