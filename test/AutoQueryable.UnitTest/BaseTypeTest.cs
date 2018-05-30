using System.Linq;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock.Entities;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace AutoQueryable.UnitTest
{
    public class BaseTypeTest
    {
        [Fact]
        public void SelectAllProducts()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable("", new AutoQueryableProfile { UseBaseType = true });
                var pp = query as IEnumerable<Product>;
                pp.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }
        
        [Fact]
        public void CreateAqWithUseBaseTypeAndUnSelectable_Query_CheckIfResultsDoesNotContainsUnselectabe()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable("namecontains=product", new AutoQueryableProfile
                {
                    UseBaseType = true,
                    DefaultToTake = int.MaxValue,
                    UnselectableProperties = typeof(Product).GetProperties().Where(p => p.Name != "Name").Select(p => p.Name).ToArray()
                });
                var products = (query as IEnumerable<Product>)?.ToList();
                products.Should().NotBeNull();
                products.Should().NotContain(p => p.Color != null || p.ProductId != 0);
                products.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }
    }
}