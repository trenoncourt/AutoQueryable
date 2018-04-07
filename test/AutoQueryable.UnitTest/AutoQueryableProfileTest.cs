using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.UnitTest
{
    [TestClass]
    public class AutoQueryableProfileTest
    {
        [TestMethod]
        public void AllowOnlyOneClause()
        {
            using (Mock.AutoQueryableContext context = new UnitTest.Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("select=name&top=10", new AutoQueryableProfile
                {
                    AllowedClauses = ClauseType.Select
                }) as IEnumerable<dynamic>).ToList();

                Assert.AreEqual(DataInitializer.ProductSampleCount, query.Count());
                var first = query.First();

                int propertiesCount = ((Type)first.GetType()).GetProperties().Count();
                Assert.IsTrue(1 == propertiesCount);
                
                string name = first.GetType().GetProperty("name").GetValue(first);
                Assert.IsNotNull(name);
            }
        }

        [TestMethod]
        public void AllowMultipleClauses()
        {
            using (Mock.AutoQueryableContext context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable("select=productId&top=10&skip=100", new AutoQueryableProfile
                {
                    AllowedClauses = ClauseType.Select | ClauseType.Top
                }) as IEnumerable<dynamic>).ToList();

                Assert.AreEqual(10, query.Count());
                var first = query.First();

                int propertiesCount = ((Type)first.GetType()).GetProperties().Count();
                Assert.IsTrue(1 == propertiesCount);

                int productid = first.GetType().GetProperty("productId").GetValue(first);
                Assert.IsTrue(1 == productid);
            }
        }
    }
}
