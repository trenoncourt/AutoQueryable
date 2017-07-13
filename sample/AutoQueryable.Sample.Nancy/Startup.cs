using System;
using System.Collections.Generic;
using AutoQueryable.Sample.Nancy.Contexts;
using AutoQueryable.Sample.Nancy.Entities;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace AutoQueryable.Sample.Nancy
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());

            using (var context = new AutoQueryableContext())
                Seed(context);
        }

        private void Seed(AutoQueryableContext context)
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
