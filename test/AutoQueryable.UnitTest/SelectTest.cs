using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Dtos;
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
                var query = context.Product.AutoQueryable("") as IQueryable<object>;
                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithSelectProjection()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=name,productcategory.name") as IQueryable<object>;

                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategoryname"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithNameAndColor()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=name,color") as IQueryable<object>;
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
                var query = context.Product.AutoQueryable("select=Name,COLOR") as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "Name"));
                Assert.IsTrue(properties.Any(p => p.Name == "COLOR"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithUnselectableProperties()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("", new AutoQueryableProfile { UnselectableProperties = new[] { "productid", "rowguid" } }) as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.IsFalse(properties.Any(p => p.Name == "ProductId"));
                Assert.IsFalse(properties.Any(p => p.Name == "Rowguid"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllWithNameAndColorWithUnselectableProperties()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=Name,COLOR", new AutoQueryableProfile {UnselectableProperties = new []{ "color" }}) as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 1);

                Assert.IsTrue(properties.Any(p => p.Name == "Name"));
                Assert.IsFalse(properties.Any(p => p.Name == "COLOR"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllWithInclude()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("top=50&select=name,SalesOrderDetail,productcategory", new AutoQueryableProfile {UnselectableProperties = new []{ "color" }}) as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 3);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "SalesOrderDetail"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));

                Assert.AreEqual(query.Count(), 50);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithDtoProjection()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.Select(p => new ProductDto
                {
                    Name = p.Name
                }).AutoQueryable("") as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 1);

                Assert.IsTrue(properties.Any(p => p.Name == "Name"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithDtoProjectionAndSelectProjection()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.Select(p => new ProductDto
                {
                    Name = p.Name,
                    Category = new ProductCategoryDto
                    {
                        Name = p.ProductCategory.Name
                    }
                }).AutoQueryable("select=name,category.name") as IQueryable<object>;

                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "categoryname"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithNameAndColorWithDtoProjection()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.Select(p => new
                {
                    p.Name,
                    p.Color,
                    categoryName = p.ProductCategory.Name
                }).AutoQueryable("select=name,color,categoryName") as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 3);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "color"));
                Assert.IsTrue(properties.Any(p => p.Name == "categoryName"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithNameAndColorIgnoreCaseWithDtoProjection()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.Select(p => new
                {
                    p.Name,
                    p.Color,
                    categoryName = p.ProductCategory.Name
                }).AutoQueryable("select=Name,COLOR") as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "Name"));
                Assert.IsTrue(properties.Any(p => p.Name == "COLOR"));

                Assert.AreEqual(query.Count(), 10000);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithUnselectablePropertiesWithDtoProjection()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.Select(p => new
                {
                    p.ProductId,
                    p.Rowguid,
                    p.Name,
                    p.Color,
                    categoryName = p.ProductCategory.Name
                }).AutoQueryable("", new AutoQueryableProfile { UnselectableProperties = new[] { "productid", "rowguid" } }) as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.IsFalse(properties.Any(p => p.Name == "ProductId"));
                Assert.IsFalse(properties.Any(p => p.Name == "Rowguid"));

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