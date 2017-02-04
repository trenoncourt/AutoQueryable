using System.Linq;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
using AutoQueryable.Sample.EfCore.Contexts;
using AutoQueryable.Sample.EfCore.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AutoQueryable.Sample.EfCore.Controllers
{
    [Route("api/products")]
    public class ProductController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <example>http://localhost:5000/api/products</example>
        /// <example>http://localhost:5000/api/products?select=name&top=50&skip=10</example>
        /// <param name="context"></param>
        /// <returns></returns>
        [AutoQueryable]
        [HttpGet]
        public IQueryable Get([FromServices] AutoQueryableContext context)
        {
            return context.Product;
        }

        /// <summary>
        /// </summary>
        /// <example>http://localhost:5000/api/products/with_dto_projection</example>
        /// <example>http://localhost:5000/api/products/with_dto_projection?select=name,category.name</example>
        /// <param name="context"></param>
        /// <returns></returns>
        [AutoQueryable]
        [HttpGet("with_dto_projection")]
        public IQueryable GetWithDtoProjection([FromServices] AutoQueryableContext context)
        {
            return context.Product.Select(p => new ProductDto
            {
                Name = p.Name,
                Category = new ProductCategoryDto
                {
                    Name = p.ProductCategory.Name
                }
            });
        }
    }
}
