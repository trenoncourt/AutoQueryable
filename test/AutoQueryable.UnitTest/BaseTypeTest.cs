using System;
using System.Collections.Generic;
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
        private readonly SimpleQueryStringAccessor _queryStringAccessor;
        private IAutoQueryableProfile _profile;
        private readonly IAutoQueryableContext _autoQueryableContext;

        public BaseTypeTest()
        {
            var settings = new AutoQueryableSettings {DefaultToTake = 10};
            _profile = new AutoQueryableProfile(settings);
            _queryStringAccessor = new SimpleQueryStringAccessor();
            var selectClauseHandler = new DefaultSelectClauseHandler();
            var orderByClauseHandler = new DefaultOrderByClauseHandler();
            var wrapWithClauseHandler = new DefaultWrapWithClauseHandler();
            var clauseMapManager = new ClauseMapManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler);
            var clauseValueManager = new ClauseValueManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler, _profile);
            var criteriaFilterManager = new CriteriaFilterManager();
            _autoQueryableContext = new AutoQueryableContext(new AutoQueryHandler( _queryStringAccessor,criteriaFilterManager ,clauseMapManager ,clauseValueManager, _profile));
        }

        [Fact]
        public void SelectAllProducts()
        {
            using (var context = new AutoQueryableDbContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }
        
        [Fact]
        public void CreateAqWithUseBaseTypeAndUnSelectable_Query_CheckIfResultsDoesNotContainsUnselectabe()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("namecontains:i=product");
                DataInitializer.InitializeSeed(context);
                _profile.UseBaseType = true;
                _profile.UnselectableProperties = typeof(Product).GetProperties().Where(p => p.Name != "Name")
                    .Select(p => p.Name).ToArray();
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var products = (query as IEnumerable<Product>)?.ToList();
                products.Should().NotBeNull();
                products.Should().NotContain(p => p.Color != null || p.ProductId != 0);
                products.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }
    }
}