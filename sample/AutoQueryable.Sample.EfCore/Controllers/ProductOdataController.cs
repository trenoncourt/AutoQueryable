using System.Linq;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
using AutoQueryable.Sample.EfCore.Contexts;
using Microsoft.AspNetCore.Mvc;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Sample.EfCore.Controllers
{
    [Route("odata/products")]
    public class ProductOdataController
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <example>http://localhost:5000/api/products</example>
        /// <example>http://localhost:5000/api/products?select=name&top=50&skip=10</example>
        /// <param name="context"></param>
        /// <returns></returns>
        //[AutoQueryable(ProviderType = ProviderType.OData)]
        [HttpGet]
        public IQueryable Get([FromServices] AutoQueryableDbContext context)
        {
            return context.Product;
        }
    }
}