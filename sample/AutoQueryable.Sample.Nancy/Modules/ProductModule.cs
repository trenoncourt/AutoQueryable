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
            this.Get("/", args => FormatterExtensions.AsJson(Response, context.Product.AutoQueryable(Context.Request.Url.Query)));

            Get("/withfilter", args =>
            {
                After.AutoQueryable(Context, context.Product);
                return "";
            });
        }
    }
}