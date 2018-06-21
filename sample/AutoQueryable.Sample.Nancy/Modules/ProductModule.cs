using System.Collections.Generic;
using AutoQueryable.Extensions;
using Nancy;
using AutoQueryable.Core.Models;
using AutoQueryable.Nancy.Filter;
using AutoQueryable.Sample.Nancy.Contexts;

namespace AutoQueryable.Sample.Nancy.Modules
{
    public sealed class ProductModule : NancyModule
    {
        public ProductModule(AutoQueryableDbContext dbContext, IAutoQueryableContext autoQueryableContext, NancyQueryStringAccessor queryStringAccessor, NancyContext nancyContext) : base("/products")
        {
            queryStringAccessor.SetQueryString(nancyContext.Request.Url.Query);

            Get<dynamic>("/", args => Response.AsJson(dbContext.Product.AutoQueryable(autoQueryableContext).ToAutoQueryListResult(autoQueryableContext) as ICollection<object>));

            Get<dynamic>("/withfilter", args =>
            {
                After.AutoQueryable(Context, dbContext.Product, autoQueryableContext);
                return "";
            });
        }
    }
}