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
        public void SelectSkip50()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=ProductId,name,color&skip=50") as IQueryable<dynamic>;
                var first = query.First();
                Type type = first.GetType();
                int value = type.GetProperty("ProductId").GetValue(first);

                Assert.AreEqual(value, 51);
                Assert.AreEqual(query.Count(), 10000 - 50);
            }
        }

        [TestMethod]
        public void SelectTake50()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=ProductId,name,color&take=50") as IQueryable<dynamic>;
                Assert.AreEqual(query.Count(), 50);
            }
        }

        [TestMethod]
        public void SelectSkipAndTake50()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=ProductId,name,color&skip=50&take=50") as IQueryable<dynamic>;
                var first = query.First();
                Type type = first.GetType();
                int value = type.GetProperty("ProductId").GetValue(first);

                Assert.AreEqual(value, 51);
                Assert.AreEqual(query.Count(), 50);
            }
        }

        [TestMethod]
        public void SelectOrderByColor()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=name,color&orderby=color") as IEnumerable<dynamic>).ToList();
                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(9998).First();

                Type type = first.GetType();
                string firstValue = type.GetProperty("color").GetValue(first);
                string secondValue = type.GetProperty("color").GetValue(second);

                string lastValue = type.GetProperty("color").GetValue(last);
                string preLastValue = type.GetProperty("color").GetValue(preLast);



                Assert.AreEqual(firstValue, "black");
                Assert.AreEqual(secondValue, "black");
                Assert.AreEqual(lastValue, "red");
                Assert.AreEqual(preLastValue, "red");
            }
        }

        [TestMethod]
        public void SelectOrderById()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=productid,name,color&orderby=productid") as IEnumerable<dynamic>).ToList();
                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(9998).First();

                Type type = first.GetType();
                int firstValue = type.GetProperty("productid").GetValue(first);
                int secondValue = type.GetProperty("productid").GetValue(second);

                int lastValue = type.GetProperty("productid").GetValue(last);
                int preLastValue = type.GetProperty("productid").GetValue(preLast);



                Assert.AreEqual(firstValue, 1);
                Assert.AreEqual(secondValue, 2);
                Assert.AreEqual(lastValue, 10000);
                Assert.AreEqual(preLastValue, 9999);
            }
        }

        [TestMethod]
        public void SelectOrderByIdDesc()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=productid,name,color&orderbydesc=productid") as IEnumerable<dynamic>).ToList();
                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(9998).First();

                Type type = first.GetType();
                int firstValue = type.GetProperty("productid").GetValue(first);
                int secondValue = type.GetProperty("productid").GetValue(second);

                int lastValue = type.GetProperty("productid").GetValue(last);
                int preLastValue = type.GetProperty("productid").GetValue(preLast);



                Assert.AreEqual(firstValue, 10000);
                Assert.AreEqual(secondValue, 9999);
                Assert.AreEqual(lastValue, 1);
                Assert.AreEqual(preLastValue, 2);
            }
        }

        [TestMethod]
        public void SelectOrderByColorDesc()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=name,color&orderbydesc=color") as IEnumerable<dynamic>).ToList();
                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(9998).First();

                Type type = first.GetType();
                string firstValue = type.GetProperty("color").GetValue(first);
                string secondValue = type.GetProperty("color").GetValue(second);

                string lastValue = type.GetProperty("color").GetValue(last);
                string preLastValue = type.GetProperty("color").GetValue(preLast);



                Assert.AreEqual(firstValue, "red");
                Assert.AreEqual(secondValue, "red");
                Assert.AreEqual(lastValue, "black");
                Assert.AreEqual(preLastValue, "black");
            }
        }

        [TestMethod]
        public void SelectOrderBySellStartDate()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=SellStartDate&orderby=SellStartDate") as IEnumerable<dynamic>).ToList();
                DateTime currentDate = DateTime.MinValue;
                foreach (var product in query)
                {
                    var date = product.GetType().GetProperty("SellStartDate").GetValue(product);
                    Assert.IsTrue(date > currentDate);
                    currentDate = date;
                }
            }
        }

        [TestMethod]
        public void SelectOrderBySellStartDateDesc()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = (context.Product.AutoQueryable("select=SellStartDate&orderbydesc=SellStartDate") as IEnumerable<dynamic>).ToList();
                DateTime currentDate = DateTime.MaxValue;
                foreach (var product in query)
                {
                    var date = product.GetType().GetProperty("SellStartDate").GetValue(product);
                    Assert.IsTrue(date < currentDate);
                    currentDate = date;
                }
            }
        }

        [TestMethod]
        public void SelectFirst()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                Product product = context.Product.AutoQueryable("first=true");
                Assert.IsTrue(product.ProductId == 1);
            }
        }

        [TestMethod]
        public void SelectLast()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                Product product = context.Product.AutoQueryable("last=true");
                Assert.IsTrue(product.ProductId == 10000);
            }
        }



        [TestMethod]
        public void SelectFirstOrderbyIdDesc()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                Product product = context.Product.AutoQueryable("first=true&orderbydesc=productid");
                Assert.IsTrue(product.ProductId == 10000);
            }
        }

        [TestMethod]
        public void SelectAllWithSelectInclude()
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
                        SellStartDate = DateTime.Today.AddHours(8*i),
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