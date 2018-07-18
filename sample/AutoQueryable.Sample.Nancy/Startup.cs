using System;
using System.Collections.Generic;
using Autofac;
using AutoQueryable.Sample.Nancy.Contexts;
using AutoQueryable.Sample.Nancy.Entities;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions.TinyIocContainer;
using AutoQueryable.Nancy.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nancy;
using Nancy.Bootstrappers.Autofac;
using Nancy.Configuration;
using Nancy.TinyIoc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AutoQueryable.Sample.Nancy
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder();
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; private set; }

        public void Configure(IApplicationBuilder app)
        {
            // Register the Autofac middleware FIRST. This also adds
            // Autofac-injected middleware registered with the container.
            
            app.UseOwin().UseNancy(options =>
            {
                options.Bootstrapper = new Bootstrapper(app.ApplicationServices);
            });

            using (var context = new AutoQueryableDbContext()) Seed(context);
        }
        
        public class Bootstrapper : DefaultNancyBootstrapper
        {
            readonly IServiceProvider _serviceProvider;

            public Bootstrapper(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public override void Configure(INancyEnvironment environment)
            {
                environment.Tracing(true, true);
            }

            protected override void ConfigureApplicationContainer(TinyIoCContainer container)
            {
                base.ConfigureApplicationContainer(container);
                container.Register(_serviceProvider.GetService<ILoggerFactory>());
                container.RegisterAutoQueryable();
            }
        }
        
        private void Seed(AutoQueryableDbContext context)
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
