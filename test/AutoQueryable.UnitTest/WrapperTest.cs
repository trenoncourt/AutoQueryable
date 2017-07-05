using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using System.Reflection;

namespace AutoQueryable.UnitTest
{
    [TestClass]
    public class WrapperTest
    { 

        [TestMethod]
        public void CountAll()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("wrapwith=count") as object;
            }
        }

        [TestMethod]
        public void WrapWithTotalCount()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                dynamic query = context.Product.AutoQueryable("wrapwith=total-count") as ExpandoObject;
                var totalCount = query.TotalCount;
                Assert.AreEqual(totalCount, DataInitializer.ProductSampleCount);
            }
        }

        [TestMethod]
        public void NextLinkWithoutSkip()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                dynamic query = context.Product.AutoQueryable("top=20&wrapwith=next-link") as ExpandoObject;
                string nextLink = query.NextLink;
                Assert.IsTrue(nextLink.Contains("skip=20"));
            }
        }

        [TestMethod]
        public void NextLinkWithSkip()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                dynamic query = context.Product.AutoQueryable("top=20&skip=20&wrapwith=next-link") as ExpandoObject;
                string nextLink = query.NextLink;
                Assert.IsTrue(nextLink.Contains("skip=40"));
            }
        }

        [TestMethod]
        public void BrowseProducts20PerPage()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                string nextLink = "top=20&wrapwith=next-link";
                int i = 0;
                while(!string.IsNullOrEmpty(nextLink))
                {
                    dynamic query = context.Product.AutoQueryable(nextLink) as ExpandoObject;
                    bool isNextLinkAvailable = ((IDictionary<String, object>)query).ContainsKey("NextLink");

                    if (!isNextLinkAvailable)
                    {
                        nextLink = null;
                        continue;
                    }
                    nextLink = query.NextLink;
                    i++;
                }
                Assert.AreEqual(DataInitializer.ProductSampleCount/20, i);
            }
        }
 
    }
}