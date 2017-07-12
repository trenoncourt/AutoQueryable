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
using AutoQueryable.Helpers;

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
                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "name"));



                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }
        [TestMethod]
        public void SelectAllProductsWithSelectProjectionWithStarSelection()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=name,productcategory.*,productcategory.name") as IQueryable<object>;

                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "name"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ProductCategoryId"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "Rowguid"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ModifiedDate"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ParentProductCategoryId"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "Product"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ParentProductCategory"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "InverseParentProductCategory"));

                




                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }

     

       [TestMethod]
        public void SelectAllProductsWithSelectProjection1()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=name,productcategory,productcategory.name") as IQueryable<object>;

                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "name"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ProductCategoryId"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "Rowguid"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ModifiedDate"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ParentProductCategoryId"));

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }
        [TestMethod]
        public void SelectAllProductsWithSelectProjection2()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=SalesOrderDetail.LineTotal") as IQueryable<object>;

                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 1);
                Assert.IsTrue(properties.Any(p => p.Name == "SalesOrderDetail"));

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType.GenericTypeArguments[0];
                Assert.IsTrue(salesOrderDetailProperty.GetProperties().Any(x => x.Name == "LineTotal"));


                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithSelectProjection3()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=SalesOrderDetail.Product.ProductId") as IQueryable<object>;

                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 1);

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType.GenericTypeArguments[0];
                var productProperty = salesOrderDetailProperty.GetProperties().FirstOrDefault(x => x.Name == "Product");
                Assert.IsNotNull(productProperty);
                Assert.IsTrue(productProperty.PropertyType.GetProperties().Any(x => x.Name == "ProductId"));
                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }

        [TestMethod]
        public void SelectAllProductsWithSelectProjection4()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=name,productcategory.name,ProductCategory.ProductCategoryId,SalesOrderDetail.LineTotal") as IQueryable<object>;

                PropertyInfo[] properties = query.First().GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 3);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));
                Assert.IsTrue(properties.Any(p => p.Name == "SalesOrderDetail"));

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "name"));
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ProductCategoryId"));

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType.GenericTypeArguments[0];
                Assert.IsTrue(salesOrderDetailProperty.GetProperties().Any(x => x.Name == "LineTotal"));



                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }

        [TestMethod]
        public void SelectAllWithNameAndColorWithUnselectableProperties()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=Name,COLOR", new AutoQueryableProfile { UnselectableProperties = new[] { "color" } }) as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 1);

                Assert.IsTrue(properties.Any(p => p.Name == "Name"));
                Assert.IsFalse(properties.Any(p => p.Name == "COLOR"));

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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
                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount - 50);
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
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

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
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

                Type type = first.GetType();
                int firstValue = type.GetProperty("productid").GetValue(first);
                int secondValue = type.GetProperty("productid").GetValue(second);

                int lastValue = type.GetProperty("productid").GetValue(last);
                int preLastValue = type.GetProperty("productid").GetValue(preLast);



                Assert.AreEqual(firstValue, 1);
                Assert.AreEqual(secondValue, 2);
                Assert.AreEqual(lastValue, DataInitializer.ProductSampleCount);
                Assert.AreEqual(preLastValue, DataInitializer.ProductSampleCount - 1);
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
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

                Type type = first.GetType();
                int firstValue = type.GetProperty("productid").GetValue(first);
                int secondValue = type.GetProperty("productid").GetValue(second);

                int lastValue = type.GetProperty("productid").GetValue(last);
                int preLastValue = type.GetProperty("productid").GetValue(preLast);



                Assert.AreEqual(firstValue, DataInitializer.ProductSampleCount);
                Assert.AreEqual(secondValue, DataInitializer.ProductSampleCount - 1);
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
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

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
                Assert.IsTrue(product.ProductId == DataInitializer.ProductSampleCount);
            }
        }



        [TestMethod]
        public void SelectFirstOrderbyIdDesc()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                Product product = context.Product.AutoQueryable("first=true&orderbydesc=productid");
                Assert.IsTrue(product.ProductId == DataInitializer.ProductSampleCount);
            }
        }

        [TestMethod]
        public void SelectAllWithSelectInclude()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("top=50&select=name,SalesOrderDetail,productcategory", new AutoQueryableProfile { UnselectableProperties = new[] { "color" } }) as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 3);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "SalesOrderDetail"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));

                Assert.AreEqual(query.Count(), 50);
            }
        }
        [TestMethod]
        public void SelectWithIncludeNavigationProperties()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("top=50&select=name,SalesOrderDetail.Product.ProductId,productcategory", new AutoQueryableProfile { UnselectableProperties = new[] { "color" } }) as IQueryable<object>;
                var firstResult = query.First();
                PropertyInfo[] properties = firstResult.GetType().GetProperties();
                Assert.AreEqual(properties.Count(), 3);
                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsNotNull(properties.Any(p => p.Name == "SalesOrderDetail"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType.GenericTypeArguments[0];
                var productProperty = salesOrderDetailProperty.GetProperties().FirstOrDefault(x => x.Name == "Product");
                Assert.IsNotNull(productProperty);
                Assert.IsTrue(productProperty.PropertyType.GetProperties().Any(x => x.Name == "ProductId"));
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

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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
                Assert.IsTrue(properties.Any(p => p.Name == "category"));

                var categoryProperty = properties.FirstOrDefault(p => p.Name == "category");
                Assert.IsTrue(categoryProperty.PropertyType.GetProperties().Any(x => x.Name == "name"));
                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }
        [TestMethod]
        public void SelectAllProductsWithSelectProjectionWithUnselectableProperty()
        {
            using (AutoQueryableContext context = new AutoQueryableContext())
            {
                var query = context.Product.AutoQueryable("select=name,productcategory.name,ProductCategory.ProductCategoryId", new AutoQueryableProfile { UnselectableProperties = new[] { "ProductCategory.ProductCategoryId" } }) as IQueryable<object>;
                PropertyInfo[] properties = query.First().GetType().GetProperties();

                Assert.AreEqual(properties.Count(), 2);

                Assert.IsTrue(properties.Any(p => p.Name == "name"));
                Assert.IsTrue(properties.Any(p => p.Name == "productcategory"));

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                Assert.IsTrue(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "name"));
                Assert.IsFalse(productcategoryProperty.PropertyType.GetProperties().Any(x => x.Name == "ProductCategoryId"));
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

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
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

                Assert.AreEqual(query.Count(), DataInitializer.ProductSampleCount);
            }
        }


    }

}