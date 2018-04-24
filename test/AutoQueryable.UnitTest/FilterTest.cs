using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Extensions;
using FluentAssertions;
using Xunit;

namespace AutoQueryable.UnitTest
{
    public class FilterTest
    {
        [Fact]
        public void IdEquals5()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("productid=5") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(1);
                var first = query.First();
                int id = first.GetType().GetProperty("ProductId").GetValue(first);
                id.Should().Be(5);
            }
        }

        [Fact]
        public void IdEquals3Or4Or5()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("productid=3,4,5") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(3);

                foreach (var product in query)
                {
                    int id = product.GetType().GetProperty("ProductId").GetValue(product);
                    id.Should().BeOneOf(3, 4, 5);
                }
            }
        }
        [Fact]
        public void ProductCateqoryIdEquals1()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("ProductCategory.ProductCategoryId=1") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }

        [Fact]
        public void IdEquals3And4()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("productid=3&productid=4") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(0);
            }
        }

        [Fact]
        public void IdEquals3Or4And5Or6()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("productid=3,4&productid=5,6") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(0);
            }
        }

        [Fact]
        public void RowGuidEqualsGuidString()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable($"rowguid={DataInitializer.GuidString}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(DataInitializer.ProductSampleCount);
                var first = query.First();
                Guid id = first.GetType().GetProperty("Rowguid").GetValue(first);
                id.Should().Be(Guid.Parse(DataInitializer.GuidString));
            }
        }

        [Fact]
        public void ColorEqualsRed()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("color=red") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }

        [Fact]
        public void ColorEqualsRedOrBlack()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);

                var query = (context.Product.AutoQueryable("color=red,black") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SellStartDateEqualsTodayJsonFormatted()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var todayJsonFormated = DateTime.Today.ToString("yyyy-MM-dd");
                var query = (context.Product.AutoQueryable($"SellStartDate={todayJsonFormated}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(1);
                var first = query.First();
                DateTime sellStartDate = first.GetType().GetProperty("SellStartDate").GetValue(first);
                sellStartDate.Should().Be(DateTime.Today);
            }
        }

        [Fact]
        public void SellStartDateEqualsTodayOrTodayPlus8HourJsonFormatted()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var todayJsonFormated = DateTime.Today.ToString("yyyy-MM-dd");
                var todayPlus8HourJsonFormated = DateTime.Today.AddHours(8).ToString("yyyy-MM-ddThh:mm:ss");
                var query = (context.Product.AutoQueryable($"SellStartDate={todayJsonFormated},{todayPlus8HourJsonFormated}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(2);
                foreach (var product in query)
                {
                    DateTime sellStartDate = product.GetType().GetProperty("SellStartDate").GetValue(product);
                    sellStartDate.Should().BeOneOf(DateTime.Today, DateTime.Today.AddHours(8));
                }
            }
        }

        [Fact]
        public void SalesOrderDetailUnitPriceEquals2()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable("SalesOrderDetail.UnitPrice=2") as IEnumerable<dynamic>;
                query.Count().Should().Be(1);
            }
        }

        [Fact]
        public void SalesOrderDetailUnitProductIdEquals1()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable("SalesOrderDetail.Product.ProductId=1") as IEnumerable<dynamic>;
                query.Count().Should().Be(1);
            }
        }

        [Fact]
        public void DateEquals()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable($"SellStartDate={DateTime.Today.AddHours(8 * 2).ToString("o")}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(1);
                var first = query.First();
                int id = first.GetType().GetProperty("ProductId").GetValue(first);
                id.Should().Be(3);
            }
        }

        [Fact]
        public void DateLessThan()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable($"SellStartDate<{DateTime.Today.AddHours(8 * 2).ToString("o")}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(2);
            }
        }

        [Fact]
        public void DateLessThanEquals()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable($"SellStartDate<={DateTime.Today.AddHours(8 * 2).ToString("o")}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(3);
            }
        }

        [Fact]
        public void DateGreaterThan()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable($"SellStartDate>{DateTime.Today.AddHours(8 * 2).ToString("o")}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(DataInitializer.ProductSampleCount - 3);
            }
        }

        [Fact]
        public void DateGreaterThanEquals()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable($"SellStartDate>={DateTime.Today.AddHours(8 * 2).ToString("o")}") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(DataInitializer.ProductSampleCount - 2); ;
            }
        }

        [Fact]
        public void DateYearEquals2010()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                DataInitializer.AddDateTimeSeeds(context);
                var query = (context.Product.AutoQueryable("SellStartDate:Year=2010") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(1); ;
            }
        }
        [Fact]
        public void DateYearShouldNotEqual2011()
        {
            using (var context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                DataInitializer.AddDateTimeSeeds(context);
                var query = (context.Product.AutoQueryable("SellStartDate:Year=2011") as IEnumerable<dynamic>).ToList();
                query.Count.Should().Be(0); ;
            }
        }
    }
}