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
    }
}