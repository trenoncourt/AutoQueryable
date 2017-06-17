using AutoQueryable.UnitTest.Mock.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoQueryable.UnitTest.Mock
{
    public class AutoQueryableContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase();
        }
        
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductModel> ProductModel { get; set; }
        public virtual DbSet<SalesOrderDetail> SalesOrderDetail { get; set; }
    }
}