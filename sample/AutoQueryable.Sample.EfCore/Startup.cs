using System;
using System.Collections.Generic;
using Autofac;
using AutoQueryable.AspNetCore.Filter;
using AutoQueryable.AspNetCore.Swagger;
using AutoQueryable.Extensions.Autofac;
using AutoQueryable.Sample.EfCore.Contexts;
using AutoQueryable.Sample.EfCore.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog.Core;
using Swashbuckle.AspNetCore.Swagger;
using ILogger = Serilog.ILogger;

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
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.AddAutoQueryable();
            });

            services
                .AddDbContext<AutoQueryableDbContext>(options => options.UseInMemoryDatabase("InMemory"));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType(typeof(HttpContextAccessor)).As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterAutoQueryable();
            builder.RegisterType<AspNetCoreQueryStringAccessor>().AsImplementedInterfaces().InstancePerLifetimeScope();
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
