using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoQueryable.UnitTest
{
    [TestClass]
    public static class DataInitializer
    {
        public static readonly string GuidString = "62559CB0-1EEF-4256-958E-AE4B95974F4E";
        public static readonly int ProductSampleCount = 1000;
        public static readonly int DefaultToTakeCount = 10;
        //[AssemblyInitialize()] 
        public static void InitializeSeed(AutoQueryableDbContext dbContext)
        {
            //using (AutoQueryableContext context = new AutoQueryableContext())
            //{
                if (dbContext.Product.Any())
                {
                    return;
                }
                var fourthCategory = new ProductCategory
                {
                    Name = "fourth"
                };
                var thirdCategory = new ProductCategory
                {
                    Name = "third",
                    ParentProductCategory = fourthCategory
                };
                var secondCategory = new ProductCategory
                {
                    Name = "second",
                    ParentProductCategory = thirdCategory
                };
                var redCategory = new ProductCategory
                {
                    Name = "red",
                    ParentProductCategory = secondCategory
                };
                var blackCategory = new ProductCategory
                {
                    Name = "black",
                    ParentProductCategory = secondCategory
                };
                var model1 = new ProductModel
                {
                    Name = "Model 1"
                };
                for (var i = 0; i < ProductSampleCount; i++)
                {

                    dbContext.Product.Add(new Product
                    {
                        Color = i % 2 == 0 ? "red" : "black",
                        ProductCategory = i % 2 == 0 ? redCategory : blackCategory,
                        ProductModel = model1,
                        ListPrice = i,
                        Name = i % 2 == 0 ? null : $"Product {i}",
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
                        },
                        ProductExtension = i % 2 == 0 ? new ProductExtension { Name = "ext" } : new ProductExtension { Name = "ext", IsDeleted = true }
                    });
                }
                dbContext.SaveChanges();
            //}
        }
        public static void AddDateTimeSeeds(AutoQueryableDbContext dbContext)
        {
            dbContext.Product.Add(new Product
            {
                Name = "TestIn2010",
                SellStartDate = new DateTime(2010, DateTime.Today.Month, DateTime.Today.Day)
            });
            dbContext.SaveChanges();
        }
    }
}
