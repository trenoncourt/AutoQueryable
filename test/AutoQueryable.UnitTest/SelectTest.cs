using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Extensions;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoQueryable.UnitTest
{
    [TestClass]
    public class SelectTest
    {
        [TestMethod]
        public void SelectAllProducts()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("") as IQueryable<Object>;
                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithNameAndColor()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=name,color") as IQueryable<Object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "color"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithNameAndColorIgnoreCase()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=Name,COLOR") as IQueryable<Object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "Name"));
                Assert.IsTrue(properties.Any(p => p.Name == "COLOR"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [ClassInitialize]
        public static void InitializeSeed(TestContext testContext)
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
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
                        Rowguid = Guid.NewGuid(),
                        Size = i % 3 == 0 ? "L" : i % 2 == 0 ? "M" : "S",
                        SellStartDate = DateTime.Today,
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