using AutoQueryable.Sample.Nancy.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoQueryable.Sample.Nancy.Contexts
{
    public class AutoQueryableDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("ProductsInMemory");
        }

        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductModel> ProductModel { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetail { get; set; }
    }
}