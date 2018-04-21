using AutoQueryable.Extensions;
using Nancy;
using AutoQueryable.Nancy.Filter;
using AutoQueryable.Sample.Nancy.Contexts;

namespace AutoQueryable.Sample.Nancy.Modules
{
    public sealed class ProductModule : NancyModule
    {
        public ProductModule(AutoQueryableContext context) : base("/products")
        {
            this.Get("/", args => FormatterExtensions.AsJson(this.Response, context.Product.AutoQueryable(this.Context.Request.Url.Query)));

            this.Get("/withfilter", args =>
            {
                this.After.AutoQueryable(this.Context, context.Product);
                return "";
            });
        }
    }
}