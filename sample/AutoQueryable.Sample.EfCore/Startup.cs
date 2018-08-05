using System;
using System.Collections.Generic;
using AutoQueryable.AspNetCore.Filter;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
using AutoQueryable.AspNetCore.Swagger;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions.DependencyInjection;
using AutoQueryable.Sample.EfCore.Contexts;
using AutoQueryable.Sample.EfCore.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;

namespace AutoQueryable.Sample.EfCore
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddApiExplorer()
                .AddJsonFormatters(settings =>
                {
                    settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                })
                .Services
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"});
                    c.AddAutoQueryable();
                })
                .AddDbContext<AutoQueryableDbContext>(options => options.UseInMemoryDatabase("InMemory"))
                .AddAutoQueryable(settings => { settings.DefaultToTake = 10; });
        }
        
        public void Configure(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.GetService<AutoQueryableDbContext>();
            Seed(context);

            app.UseMvc();
            
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }

        private void Seed(AutoQueryableDbContext context)
        {
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
            for (var i = 0; i < 10000; i++)
            {
                context.Product.Add(new Product
                {
                    Color = i % 2 == 0 ? "red" : "black",
                    ProductCategory = i % 2 == 0 ? redCategory : blackCategory,
                    ProductModel = model1,
                    ListPrice = (decimal) (i / 5.0),
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
