using System.Linq;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AutoQueryable.UnitTest
{
    [TestClass]
    public class BaseTypeTest
    {
        [TestMethod]
        public void SelectAllProducts()
        {
            using (Mock.AutoQueryableContext context = new Mock.AutoQueryableContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable("?skip=1&take=1", new AutoQueryableProfile {UseBaseType = true});
                IEnumerable<Product> pp = query as IEnumerable<Product>;
                Assert.AreEqual(pp.Count(), DataInitializer.ProductSampleCount);
            }
        }
    }
}