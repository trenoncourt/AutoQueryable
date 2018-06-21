using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;
using AutoQueryable.UnitTest.Mock;

namespace AutoQueryable.UnitTest
{
    public class PagedResultTest
    {

        private readonly SimpleQueryStringAccessor _queryStringAccessor;
        private readonly IAutoQueryableProfile _profile;
        private readonly IAutoQueryableContext _autoQueryableContext;

        public PagedResultTest()
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

        //[Fact]
        //public void CountAll()
        //{
        //    using (var context = new AutoQueryableContext())
        //    {
        //        DataInitializer.InitializeSeed(context);
        //        var query = context.Product.AutoQueryable("wrapwith=count") as object;
        //    }
        //}

        //[Fact]
        //public void WrapWithTotalCount()
        //{
        //    using (var context = new AutoQueryableContext())
        //    {
        //        DataInitializer.InitializeSeed(context);
        //        dynamic query = context.Product.AutoQueryable("wrapwith=total-count") as ExpandoObject;
        //        var totalCount = query.TotalCount as int?;
        //        totalCount.Should().Be(DataInitializer.ProductSampleCount);
        //    }
        //}

        //[Fact]
        //public void NextLinkWithoutSkip()
        //{
        //    using (var context = new AutoQueryableContext())
        //    {
        //        DataInitializer.InitializeSeed(context);
        //        dynamic query = context.Product.AutoQueryable("top=20&wrapwith=next-link") as ExpandoObject;
        //        string nextLink = query.NextLink;
        //        nextLink.Should().Contain("skip=20");
        //    }
        //}

        //[Fact]
        //public void NextLinkWithSkip()
        //{
        //    using (var context = new AutoQueryableContext())
        //    {
        //        DataInitializer.InitializeSeed(context);
        //        dynamic query = context.Product.AutoQueryable("top=20&skip=20&wrapwith=next-link") as ExpandoObject;
        //        string nextLink = query.NextLink;
        //        nextLink.Should().Contain("skip=40");
        //    }
        //}

        //[Fact]
        //public void BrowseProducts20PerPage()
        //{
        //    using (var context = new AutoQueryableContext())
        //    {
        //        DataInitializer.InitializeSeed(context);
        //        var nextLink = "top=20&wrapwith=next-link";
        //        var i = 0;
        //        while (!string.IsNullOrEmpty(nextLink))
        //        {
        //            dynamic query = context.Product.AutoQueryable(nextLink) as ExpandoObject;
        //            var isNextLinkAvailable = ((IDictionary<String, object>)query).ContainsKey("NextLink");

        //            if (!isNextLinkAvailable)
        //            {
        //                nextLink = null;
        //                continue;
        //            }
        //            nextLink = query.NextLink;
        //            i++;
        //        }

        //        i.Should().Be(DataInitializer.ProductSampleCount / 20);
        //    }
        //}

    }
}