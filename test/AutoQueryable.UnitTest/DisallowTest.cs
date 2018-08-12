using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using FluentAssertions;
using Xunit;

namespace AutoQueryable.UnitTest
{
    public class DisallowTest
    {
        
        private readonly SimpleQueryStringAccessor _queryStringAccessor;
        private readonly IAutoQueryableProfile _profile;
        private readonly IAutoQueryableContext _autoQueryableContext;
        private readonly int _baseProductPropertiesCount = 17;

        public DisallowTest()
        {
            var settings = new AutoQueryableSettings {DefaultToTake = 10};
            _profile = new AutoQueryableProfile(settings);
            _queryStringAccessor = new SimpleQueryStringAccessor();
            var selectClauseHandler = new DefaultSelectClauseHandler();
            var orderByClauseHandler = new DefaultOrderByClauseHandler();
            var wrapWithClauseHandler = new DefaultWrapWithClauseHandler();
            var clauseMapManager = new ClauseMapManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler, _profile);
            var clauseValueManager = new ClauseValueManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler, _profile);
            var criteriaFilterManager = new CriteriaFilterManager();
            var defaultAutoQueryHandler = new AutoQueryHandler(_queryStringAccessor,criteriaFilterManager ,clauseMapManager ,clauseValueManager, _profile);
            _autoQueryableContext = new AutoQueryableContext(defaultAutoQueryHandler);
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("select=name")]
        [InlineData("select=productId&productId=1")]
        public void DisalowSelectClause_QueryWithSelect_ResultsShouldCountainsAllProperties(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.Select;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();
                
                properties.Length.Should().Be(_baseProductPropertiesCount);
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("productId=1")]
        [InlineData("select=productId&productId=1")]
        public void DisalowFilterClause_QueryWithFilter_ResultsCountShouldBeDefaultToTake(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.Filter;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("first=true")]
        [InlineData("first=true&select=name")]
        [InlineData("first=true&select=name&productId=1")]
        public void DisalowFirstClause_QueryWithFirst_ResultsShouldBeEnumerable(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.First;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                query.Should().NotBeNullOrEmpty();
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("last=true")]
        [InlineData("last=true&select=name")]
        [InlineData("last=true&select=name&productId=1")]
        public void DisalowLastClause_QueryWithLast_ResultsShouldBeEnumerable(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.Last;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                query.Should().NotBeNullOrEmpty();
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("groupby=productId")]
        [InlineData("groupby=productId&select=name")]
        [InlineData("groupby=productId&select=name&productId=1")]
        public void DisalowGroupByClause_QueryWithGroupBy_ResultsShouldBeNonGrouped(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.GroupBy;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                query.Should().NotBeNullOrEmpty();
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("orderby=color")]
        [InlineData("orderby=+color")]
        [InlineData("orderby=-color")]
        [InlineData("orderby=color&select=ProductId")]
        [InlineData("orderby=color&select=ProductId&productId=1")]
        public void DisalowOrderByClause_QueryWithOrderBy_ResultsShouldBeOrdered(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.OrderBy;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var first = query.First();
                    first.GetType().GetProperty("ProductId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).GetValue(first).As<int>().Should().Be(1);;
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("skip=10")]
        [InlineData("skip=10&top=10&select=ProductId")]
        [InlineData("skip=10&productId=1")]
        public void DisalowSkipClause_QueryWithSkip_FirstResultIdShouldBe1(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.Skip;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var first = query.First();
                    first.GetType().GetProperty("ProductId", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public).GetValue(first).As<int>().Should().Be(1);;
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("top=5")]
        [InlineData("skip=10&top=1&select=ProductId")]
        public void DisalowTopClause_QueryWithTop_ResultsCountShouldBeDefaultToTake(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.DisAllowedClauses = ClauseType.Top;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                
                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }
        
        [Theory]
        [InlineData("")]
        [InlineData("first=true")]
        [InlineData("last=true")]
        [InlineData("groupBy=color")]
        [InlineData("wrapWith=count")]
        [InlineData("productId=1")]
        [InlineData("select=productId&productId=1")]
        public void AllowOnlyFilterClause_QueryWithSelect_ResultsShouldCountainsAllProperties(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.AllowedClauses = ClauseType.Filter;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(_baseProductPropertiesCount);
            }
        }
        
        [Theory]
        [InlineData("productId=1")]
        [InlineData("productId=1&first=true")]
        [InlineData("productId=1&last=true")]
        [InlineData("productId=1&groupBy=color")]
        [InlineData("productId=1&wrapWith=count")]
        public void AllowOnlyFilterClause_QueryWithFilter_ResultsCountShouldBe1(string queryString)
        {
            using (var context = new AutoQueryableDbContext())
            {
                _profile.AllowedClauses = ClauseType.Filter;
                _queryStringAccessor.SetQueryString(queryString);

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                query.Count().Should().Be(1);
            }
        }
    }
}