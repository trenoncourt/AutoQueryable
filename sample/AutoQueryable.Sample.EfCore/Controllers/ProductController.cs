using System.Linq;
using AutoQueryable.AspNetCore.Filter.FilterAttributes;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using AutoQueryable.Sample.EfCore.Contexts;
using AutoQueryable.Sample.EfCore.Dtos;
using AutoQueryable.Sample.EfCore.Entities;
using Microsoft.AspNetCore.Mvc;


namespace AutoQueryable.Sample.EfCore.Controllers
{
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IAutoQueryableContext _autoQueryableContext;
        private readonly IAutoQueryableProfile _profile;

        public ProductController(IAutoQueryableContext autoQueryableContext, IAutoQueryableProfile profile)
        {
            _autoQueryableContext = autoQueryableContext;
            _profile = profile;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <example>http://localhost:5000/api/products</example>
        /// <example>http://localhost:5000/api/products?select=name&top=50&skip=10</example>
        /// <param name="context"></param>
        /// <returns></returns>
        [TypeFilter(typeof(AutoQueryableAttribute))]
        [HttpGet]
        public IQueryable Get([FromServices] AutoQueryableDbContext context)
        {
            _profile.MaxToTake = 10;
            return context.Product;
        }

        /// <summary>
        /// </summary>
        /// <example>http://localhost:5000/api/products/with_dto_projection</example>
        /// <example>http://localhost:5000/api/products/with_dto_projection?select=name,category.name</example>
        /// <param name="context"></param>
        /// <returns></returns>
        //[AutoQueryable]
        [HttpGet("with_dto_projection")]
        public IQueryable GetWithDtoProjection([FromServices] AutoQueryableDbContext context)
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

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [HttpGet("disallow")]
        public dynamic GetWithNotAllowedClauses([FromServices] AutoQueryableDbContext context)
        {
           _profile.AllowedClauses = ClauseType.Select | ClauseType.Skip | ClauseType.OrderBy | ClauseType.OrderByDesc | ClauseType.WrapWith | ClauseType.Filter;
            _profile.MaxToTake = 5;

//                MaxToSkip = 5,
//                MaxDepth = 2,
////                    SelectableProperties = new[] { "name", "color" },
//                DisAllowedConditions = ConditionType.Contains | ConditionType.Less,
//                SortableProperties = new []{"color"},
//                AllowedWrapperPartType = WrapperPartType.Count
//            };
            return context.Product.AutoQueryable(_autoQueryableContext);
        }
    }
}
