using System;
using System.Linq;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using AutoQueryable.Helpers;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace AutoQueryable.UnitTest
{
    public class BaseTypeTest
    {
        private SimpleQueryStringAccessor _queryStringAccessor;
        private IAutoQueryableProfile _profile;
        private IAutoQueryableContext _autoQueryableContext;

        public BaseTypeTest()
        {
            _profile = new AutoQueryableProfile();
            _queryStringAccessor = new SimpleQueryStringAccessor();
            var selectClauseHandler = new DefaultSelectClauseHandler();
            var orderByClauseHandler = new DefaultOrderByClauseHandler();
            var wrapWithClauseHandler = new DefaultWrapWithClauseHandler();
            var clauseMapManager = new ClauseMapManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler);
            var clauseValueManager = new ClauseValueManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler);
            var criteriaFilterManager = new CriteriaFilterManager();
            _autoQueryableContext = new AutoQueryableContext(_profile, new AutoQueryHandler( _queryStringAccessor,criteriaFilterManager ,clauseMapManager ,clauseValueManager));
        }

        [Fact]
        public void SelectAllProducts()
        {
            using (var context = new AutoQueryableDbContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext);
                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }
        
        //[Fact]
        //public void CreateAqWithUseBaseTypeAndUnSelectable_Query_CheckIfResultsDoesNotContainsUnselectabe()
        //{
        //    using (var context = new Mock.AutoQueryableContext())
        //    {
        //        DataInitializer.InitializeSeed(context);
        //        var query = context.Product.AutoQueryable("namecontains=product", new AutoQueryableProfile
        //        {
        //            UseBaseType = true,
        //            DefaultToTake = int.MaxValue,
        //            UnselectableProperties = typeof(Product).GetProperties().Where(p => p.Name != "Name").Select(p => p.Name).ToArray()
        //        });
        //        var products = (query as IEnumerable<Product>)?.ToList();
        //        products.Should().NotBeNull();
        //        products.Should().NotContain(p => p.Color != null || p.ProductId != 0);
        //        products.Count().Should().Be(DataInitializer.ProductSampleCount);
        //    }
        //}
    }
}