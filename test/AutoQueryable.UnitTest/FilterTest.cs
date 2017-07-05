using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoQueryable.UnitTest
{
    [TestClass]
    public class FilterTest
    {
       
        [TestMethod]
        public void IdEquals5()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("productid=5") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(query.Count(), 1);
                var first = query.First();
                int id = first.GetType().GetProperty("ProductId").GetValue(first);
                Assert.IsTrue(id == 5);
            }
        }

        [TestMethod]
        public void IdEquals3Or4Or5()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("productid=3,4,5") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(query.Count(), 3);

                foreach (var product in query)
                {
                    int id = product.GetType().GetProperty("ProductId").GetValue(product);
                    Assert.IsTrue(id == 3 || id == 4 || id == 5);
                }
            }
        }
        [TestMethod]
        public void ProductCateqoryIdEquals1()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("ProductCategory.ProductCategoryId=1") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount / 2);
            }
        }

        [TestMethod]
        public void IdEquals3And4()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("productid=3&productid=4") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(query.Count(), 0);
            }
        }

        [TestMethod]
        public void IdEquals3Or4And5Or6()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("productid=3,4&productid=5,6") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(query.Count(), 0);
            }
        }

        [TestMethod]
        public void RowGuidEqualsGuidString()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable($"rowguid={DataInitializer.GuidString}") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(DataInitializer.ProductSampleCount, query.Count());
                var first = query.First();
                Guid id = first.GetType().GetProperty("Rowguid").GetValue(first);
                Assert.IsTrue(id == Guid.Parse(DataInitializer.GuidString));
            }
        }

        [TestMethod]
        public void ColorEqualsRed()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("color=red") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount / 2);
            }
        }

        [TestMethod]
        public void ColorEqualsRedOrBlack()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("color=red,black") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }

        [TestMethod]
        public void SellStartDateEqualsTodayJsonFormatted()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                string todayJsonFormated = DateTime.Today.ToString("yyyy-MM-dd");
                var query = (context.Product.AutoQueryable($"SellStartDate={todayJsonFormated}") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(1, query.Count);
                var first = query.First();
                DateTime sellStartDate = first.GetType().GetProperty("SellStartDate").GetValue(first);
                Assert.IsTrue(sellStartDate == DateTime.Today);
            }
        }

        [TestMethod]
        public void SellStartDateEqualsTodayOrTodayPlus8HourJsonFormatted()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                string todayJsonFormated = DateTime.Today.ToString("yyyy-MM-dd");
                string todayPlus8HourJsonFormated = DateTime.Today.AddHours(8).ToString("yyyy-MM-ddThh:mm:ss");
                var query = (context.Product.AutoQueryable($"SellStartDate={todayJsonFormated},{todayPlus8HourJsonFormated}") as IEnumerable<dynamic>).ToList();
                Assert.AreEqual(2, query.Count);
                foreach (var product in query)
                {
                    DateTime sellStartDate = product.GetType().GetProperty("SellStartDate").GetValue(product);
                    Assert.IsTrue(sellStartDate == DateTime.Today || sellStartDate == DateTime.Today.AddHours(8));
                }
            }
        }
        [TestMethod]
        public void SalesOrderDetailUnitPriceEquals2()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("SalesOrderDetail.UnitPrice=2") as IEnumerable<dynamic>;
                Assert.AreEqual(query.Count(), 1);
            }
        }
        [TestMethod]
        public void SalesOrderDetailUnitProductIdEquals1()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("SalesOrderDetail.Product.ProductId=1") as IEnumerable<dynamic>;
                Assert.AreEqual(query.Count(), 1);
            }
        } 
 
    }
}