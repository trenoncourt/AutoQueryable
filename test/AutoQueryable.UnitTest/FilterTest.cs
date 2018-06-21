using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Entities;
using FluentAssertions;
using Xunit;

namespace AutoQueryable.UnitTest
{
    public class FilterTest
    {
        private readonly SimpleQueryStringAccessor _queryStringAccessor;
        private readonly IAutoQueryableProfile _profile;
        private readonly IAutoQueryableContext _autoQueryableContext;

        public FilterTest()
        {
            _profile = new AutoQueryableProfile();

            _profile.DefaultToTake = 0;

            _queryStringAccessor = new SimpleQueryStringAccessor();
            var selectClauseHandler = new DefaultSelectClauseHandler();
            var orderByClauseHandler = new DefaultOrderByClauseHandler();
            var wrapWithClauseHandler = new DefaultWrapWithClauseHandler();
            var clauseMapManager = new ClauseMapManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler);
            var clauseValueManager = new ClauseValueManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler);
            var criteriaFilterManager = new CriteriaFilterManager();
            var defaultAutoQueryHandler = new AutoQueryHandler(_queryStringAccessor,criteriaFilterManager ,clauseMapManager ,clauseValueManager);
            _autoQueryableContext = new AutoQueryableContext(_profile, defaultAutoQueryHandler);
        }
        [Fact]
        public void IdEquals5()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("productid=5");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(1);
                var first = query.First();
                var id = first.GetType().GetProperty("ProductId").GetValue(first);
                id.Should().Be(5);
            }
        }

        [Fact]
        public void IdEquals3Or4Or5()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("productid=3,4,5");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(3);

                foreach (var product in query)
                {
                    var id = int.Parse(product.GetType().GetProperty("ProductId").GetValue(product).ToString());
                    id.Should().BeOneOf(3, 4, 5);
                }
            }
        }
        [Fact]
        public void ProductCateqoryIdEquals1()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("ProductCategory.ProductCategoryId=1");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }

        [Fact]
        public void IdEquals3And4()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("productid=3&productid=4");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(0);
            }
        }

        [Fact]
        public void IdEquals3Or4And5Or6()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("productid=3,4&productid=5,6");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(0);
            }
        }
        [Fact]
        public void NullablePropertyEquals()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("name=Product 23");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(1);
            }
        }
        [Fact]
        public void NameEqualsNull()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("name=null");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
                query.Should().OnlyContain(product => product.GetType().GetProperty("Name").GetValue(product) == null);
            }
        }
        [Fact]
        public void NameNotEqualsNull()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("name!=null");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
                query.Should().OnlyContain(product => product.GetType().GetProperty("Name").GetValue(product) != null);
            }
        }

        [Fact]
        public void NullableValueContains()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("namecontains=Product");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }

        [Fact]
        public void ContainsIgnoreCase()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("namecontains:i=proDuct");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }
        [Fact]
        public void StartsWith()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("namestartsWith=Prod");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }
        [Fact]
        public void NotStartsWith()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("nameStartsWith!=Prod");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }
        [Fact]
        public void StartsWithIgnoreCase()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("namestartsWith:i=prod");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }
        [Fact]
        public void NotStartsWithIgnoreCase()
        {
            using (var context = new AutoQueryableDbContext())
            {
                const string nameCheck = "prodUct 10";
                _queryStringAccessor.SetQueryString($"namestartsWith:i!={nameCheck}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                query.Should().OnlyContain(product => 
                    product.GetType().GetProperty("Name").GetValue(product) == null 
                    || !product.GetType().GetProperty("Name").GetValue(product).ToString().StartsWith(nameCheck, StringComparison.OrdinalIgnoreCase));
            }
        }
        [Fact]
        public void NullableValueEndsWith()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("nameEndsWith=999");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(1);
            }
        }
        [Fact]
        public void EndsWithIgnoreCase()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("nameEndsWith:i=cT 999");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Count().Should().Be(1);
            }
        }
        [Fact]
        public void NotEndsWithIgnoreCase()
        {
            using (var context = new AutoQueryableDbContext())
            {
                const string nameCheck = "dUcT 100";
                _queryStringAccessor.SetQueryString($"nameEndsWith:i!={nameCheck}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                query.Should().OnlyContain(product => 
                    product.GetType().GetProperty("Name").GetValue(product) == null 
                    || !product.GetType().GetProperty("Name").GetValue(product).ToString().EndsWith(nameCheck, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact]
        public void RowGuidEqualsGuidString()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString($"rowguid={DataInitializer.GuidString}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
                var first = query.First();
                var id = first.GetType().GetProperty("Rowguid").GetValue(first);
                id.Should().Be(Guid.Parse(DataInitializer.GuidString));
            }
        }

        [Fact]
        public void ColorEqualsRed()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("color=red");
                
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.ProductSampleCount / 2);
            }
        }

        [Fact]
        public void ColorEqualsRedOrBlack()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("color=red,black");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SellStartDateEqualsTodayJsonFormatted()
        {
            using (var context = new AutoQueryableDbContext())
            {
                var todayJsonFormated = DateTime.Today.ToString("yyyy-MM-dd");
                _queryStringAccessor.SetQueryString($"SellStartDate={todayJsonFormated}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                query.Count().Should().Be(1);
                var first = query.First();
                var sellStartDate = DateTime.Parse(first.GetType().GetProperty("SellStartDate").GetValue(first).ToString());
                sellStartDate.Should().Be(DateTime.Today);
            }
        }

        [Fact]
        public void SellStartDateEqualsTodayOrTodayPlus8HourJsonFormatted()
        {
            using (var context = new AutoQueryableDbContext())
            {
                var todayJsonFormated = DateTime.Today.ToString("yyyy-MM-dd");
                var todayPlus8HourJsonFormated = DateTime.Today.AddHours(8).ToString("yyyy-MM-ddThh:mm:ss");
                _queryStringAccessor.SetQueryString($"SellStartDate={todayJsonFormated},{todayPlus8HourJsonFormated}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(2);
                foreach (var product in query)
                {
                    var sellStartDate = DateTime.Parse(product.GetType().GetProperty("SellStartDate").GetValue(product).ToString());
                    sellStartDate.Should().BeOneOf(DateTime.Today, DateTime.Today.AddHours(8));
                }
            }
        }

        [Fact]
        public void SalesOrderDetailUnitPriceEquals2()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("SalesOrderDetail.UnitPrice=2");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(1);
            }
        }

        [Fact]
        public void SalesOrderDetailUnitProductIdEquals1()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("SalesOrderDetail.Product.ProductId=1");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(1);
            }
        }

        [Fact]
        public void DateEquals()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString($"SellStartDate={DateTime.Today.AddHours(8 * 2):o}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(1);
                var first = query.First();
                var id = int.Parse(first.GetType().GetProperty("ProductId").GetValue(first).ToString());
                id.Should().Be(3);
            }
        }

        [Fact]
        public void DateLessThan()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString($"SellStartDate<{DateTime.Today.AddHours(8 * 2):o}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(2);
            }
        }

        [Fact]
        public void DateLessThanEquals()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString($"SellStartDate<={DateTime.Today.AddHours(8 * 2):o}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(3);
            }
        }

        [Fact]
        public void DateGreaterThan()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString($"SellStartDate>{DateTime.Today.AddHours(8 * 2):o}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.ProductSampleCount - 3);
            }
        }

        [Fact]
        public void DateGreaterThanEquals()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString($"SellStartDate>={DateTime.Today.AddHours(8 * 2):o}");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.ProductSampleCount - 2);
            }
        }

        [Fact]
        public void DateYearEquals2010()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("SellStartDate:Year=2010");

                DataInitializer.InitializeSeed(context);
                DataInitializer.AddDateTimeSeeds(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                

                query.Count().Should().Be(1);
            }
        }
        [Fact]
        public void DateYearShouldNotEqual2011()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("SellStartDate:Year=2011");

                DataInitializer.InitializeSeed(context);
                DataInitializer.AddDateTimeSeeds(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;

                query.Count().Should().Be(0);
            }
        }
    }
}