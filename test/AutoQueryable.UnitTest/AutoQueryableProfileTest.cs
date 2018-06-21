using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AutoQueryableProfileTest
    {
        private readonly SimpleQueryStringAccessor _queryStringAccessor;
        private readonly IAutoQueryableProfile _profile;
        private readonly IAutoQueryableContext _autoQueryableContext;

        public AutoQueryableProfileTest()
        {
            _profile = new AutoQueryableProfile();
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
        public void AllowOnlyOneClause()
        {
            using (var context = new AutoQueryableDbContext())
            {
                DataInitializer.InitializeSeed(context);
                _queryStringAccessor.SetQueryString("select=id");

                _profile.AllowedClauses = ClauseType.Select;

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;


                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
                var first = query.First();

                var propertiesCount = first.GetType().GetProperties().Length;
                propertiesCount.Should().Be(1);

                var name = first.GetType().GetProperty("id").GetValue(first);
                name.Should().NotBeNull();
            }
        }

        [Fact]
        public void AllowMultipleClauses()
        {
            using (var context = new AutoQueryableDbContext())
            {
                DataInitializer.InitializeSeed(context);

                _queryStringAccessor.SetQueryString("select=productId&top=10&skip=100");
                _profile.AllowedClauses = ClauseType.Select | ClauseType.Top;

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                query.Count().Should().Be(10);
                var first = query.First();

                var propertiesCount = first.GetType().GetProperties().Length;
                propertiesCount.Should().Be(1);

                var productid = first.GetType().GetProperty("productId").GetValue(first);
                productid.Should().Be(101);
            }
        }
    }
}
