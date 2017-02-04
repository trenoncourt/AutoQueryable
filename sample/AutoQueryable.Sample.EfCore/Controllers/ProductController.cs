using System.Linq;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
using AutoQueryable.Sample.EfCore.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace AutoQueryable.Sample.EfCore.Controllers
{
    [Route("api/products")]
    public class ProductController
    {
        [AutoQueryable]
        [HttpGet]
        public IQueryable Get([FromServices] AutoQueryableContext context)
        {
            return context.Product;
        }
    }
}
