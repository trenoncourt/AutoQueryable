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
        public static readonly string GuidString = "62559CB0-1EEF-4256-958E-AE4B95974F4E";

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
                Assert.AreEqual(totalCount, 10000);
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
                Assert.AreEqual(500, i);
            }
        }

        [ClassInitialize]
        public static void InitializeSeed(TestContext testContext)
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                if (context.Product.Any())
                {
                    return;
                }
                var redCategory = new ProductCategory
                {
                    Name = "red"
                };
                var blackCategory = new ProductCategory
                {
                    Name = "black"
                };
                var model1 = new ProductModel
                {
                    Name = "Model 1"
                };
                for (int i = 0; i < 10000; i++)
                {
                    context.Product.Add(new Product
                    {
                        Color = i % 2 == 0 ? "red" : "black",
                        ProductCategory = i % 2 == 0 ? redCategory : blackCategory,
                        ProductModel = model1,
                        ListPrice = i,
                        Name = $"Product {i}",
                        ProductNumber = Guid.NewGuid().ToString(),
                        Rowguid = Guid.Parse(GuidString),
                        Size = i % 3 == 0 ? "L" : i % 2 == 0 ? "M" : "S",
                        SellStartDate = DateTime.Today.AddHours(8 * i),
                        StandardCost = i + 1,
                        Weight = i % 32,
                        SalesOrderDetail = new List<SalesOrderDetail>
                        {
                            new SalesOrderDetail
                            {
                                LineTotal = i % 54,
                                OrderQty = 5,
                                UnitPrice = i + i,
                                UnitPriceDiscount = i + i / 2
                            },
                            new SalesOrderDetail
                            {
                                LineTotal = i + 15 % 64,
                                OrderQty = 3,
                                UnitPrice = i + i,
                                UnitPriceDiscount = i + i / 2
                            }
                        }
                    });
                }
                context.SaveChanges();
            }
        }
    }
}