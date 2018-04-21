using System;
using System.Collections.Generic;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using System.Dynamic;
using FluentAssertions;
using Xunit;

namespace AutoQueryable.UnitTest
{
    public class WrapperTest
    {

        [Fact]
        public void CountAll()
        {
            using (var context = new AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable("wrapwith=count") as object;
            }
        }

        [Fact]
        public void WrapWithTotalCount()
        {
            using (var context = new AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                dynamic query = context.Product.AutoQueryable("wrapwith=total-count") as ExpandoObject;
                var totalCount = query.TotalCount as int?;
                totalCount.Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void NextLinkWithoutSkip()
        {
            using (var context = new AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                dynamic query = context.Product.AutoQueryable("top=20&wrapwith=next-link") as ExpandoObject;
                string nextLink = query.NextLink;
                nextLink.Should().Contain("skip=20");
            }
        }

        [Fact]
        public void NextLinkWithSkip()
        {
            using (var context = new AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                dynamic query = context.Product.AutoQueryable("top=20&skip=20&wrapwith=next-link") as ExpandoObject;
                string nextLink = query.NextLink;
                nextLink.Should().Contain("skip=40");
            }
        }

        [Fact]
        public void BrowseProducts20PerPage()
        {
            using (var context = new AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var nextLink = "top=20&wrapwith=next-link";
                var i = 0;
                while (!string.IsNullOrEmpty(nextLink))
                {
                    dynamic query = context.Product.AutoQueryable(nextLink) as ExpandoObject;
                    var isNextLinkAvailable = ((IDictionary<String, object>)query).ContainsKey("NextLink");

                    if (!isNextLinkAvailable)
                    {
                        nextLink = null;
                        continue;
                    }
                    nextLink = query.NextLink;
                    i++;
                }

                i.Should().Be(DataInitializer.ProductSampleCount / 20);
            }
        }

    }
}