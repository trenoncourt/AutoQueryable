using AutoQueryable.Extensions;
using AutoQueryable.Models.Enums;
using AutoQueryable.UnitTest.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AutoQueryable.UnitTest
{
    [TestClass]
    public class AutoQueryableProfileTest
    {
        [TestMethod]
        public void AllowOnlyOneClause()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=name&top=10", new Models.AutoQueryableProfile
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
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=productId&top=10&skip=100", new Models.AutoQueryableProfile
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
