using System;
using System.Linq;
using System.Threading.Tasks;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Extensions;
using AutoQueryable.Core.Models;
using AutoQueryable.UnitTest.Mock;
using AutoQueryable.UnitTest.Mock.Dtos;
using AutoQueryable.UnitTest.Mock.Entities;
using FluentAssertions;
using Xunit;

namespace AutoQueryable.UnitTest
{
    public class SelectTest
    {
        private SimpleQueryStringAccessor _queryStringAccessor;
        private IAutoQueryableProfile _profile;
        private IAutoQueryableContext _autoQueryableContext;

        public SelectTest()
        {
            var settings = new AutoQueryableSettings {DefaultToTake = 10};
            _profile = new AutoQueryableProfile(settings);
            _queryStringAccessor = new SimpleQueryStringAccessor();
            var selectClauseHandler = new DefaultSelectClauseHandler();
            var orderByClauseHandler = new DefaultOrderByClauseHandler();
            var wrapWithClauseHandler = new DefaultWrapWithClauseHandler();
            var clauseMapManager = new ClauseMapManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler);
            var clauseValueManager = new ClauseValueManager(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler, _profile);
            var criteriaFilterManager = new CriteriaFilterManager();
            var defaultAutoQueryHandler = new AutoQueryHandler(_queryStringAccessor,criteriaFilterManager ,clauseMapManager ,clauseValueManager, _profile);
            _autoQueryableContext = new AutoQueryableContext(defaultAutoQueryHandler);
        }
        [Fact]
        public void SelectAllProducts()
        {
            using (var context = new AutoQueryableDbContext())
            {

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithSelectProjection()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,productcategory.name&top=0");

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "productcategory");

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(p => p.Name == "name");



                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithSelectProjectionWithStarSelection()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,productcategory.*,productcategory.name&top=0");

                DataInitializer.InitializeSeed(context);
                var query =
                    context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "productcategory");

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(p => p.Name == "Name");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .Contain(p => p.Name == "ProductCategoryId");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(p => p.Name == "Rowguid");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(p => p.Name == "ModifiedDate");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .Contain(p => p.Name == "ParentProductCategoryId");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(p => p.Name == "Product");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .Contain(p => p.Name == "ParentProductCategory");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .Contain(p => p.Name == "InverseParentProductCategory");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }


        [Fact]
        public void SelectAllProductsWithSelectProjection0()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=ProductCategory.Product.name,ProductCategory.Product.name,ProductCategory.Product.ProductId,ProductCategory.name&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(1);
                properties.Should().Contain(p => p.Name == "ProductCategory");
                var productCategoryProperties = properties.FirstOrDefault(p => p.Name == "ProductCategory").PropertyType
                    .GetProperties();

                productCategoryProperties.Should().Contain(x => x.Name == "name");
                productCategoryProperties.Should().Contain(x => x.Name == "Product");
                var productProperties = productCategoryProperties.FirstOrDefault(p => p.Name == "Product").PropertyType
                    .GenericTypeArguments[0].GetProperties();

                productProperties.Should().Contain(x => x.Name == "name");
                productProperties.Should().Contain(x => x.Name == "ProductId");
                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithSelectProjection1()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,productcategory,productcategory.name&top=0");

                DataInitializer.InitializeSeed(context);
                var query =
                    context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "productcategory");

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "Name");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .Contain(x => x.Name == "ProductCategoryId");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "Rowguid");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "ModifiedDate");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .Contain(x => x.Name == "ParentProductCategoryId");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithSelectProjection2()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=SalesOrderDetail.LineTotal&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(1);
                properties.Should().Contain(p => p.Name == "SalesOrderDetail");

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType
                    .GenericTypeArguments[0];
                salesOrderDetailProperty.GetProperties().Should().Contain(x => x.Name == "LineTotal");


                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithSelectProjection3()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=SalesOrderDetail.Product.ProductId&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(1);

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType
                    .GenericTypeArguments[0];
                var productProperty = salesOrderDetailProperty.GetProperties().FirstOrDefault(x => x.Name == "Product");
                productProperty.Should().NotBeNull();
                productProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "ProductId");
                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithSelectProjection4()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,productcategory.name,ProductCategory.ProductCategoryId,SalesOrderDetail.LineTotal&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(3);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "productcategory");
                properties.Should().Contain(p => p.Name == "SalesOrderDetail");

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "name");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .Contain(x => x.Name == "ProductCategoryId");

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType
                    .GenericTypeArguments[0];
                salesOrderDetailProperty.GetProperties().Should().Contain(x => x.Name == "LineTotal");



                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithNameAndColor()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,color&top=0");

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "color");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithNameAndColorIgnoreCase()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=Name,COLOR&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "Name");
                properties.Should().Contain(p => p.Name == "COLOR");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithUnselectableProperties()
        {
            using (var context = new AutoQueryableDbContext())
            {
               _profile.UnselectableProperties = new[] { "productid", "rowguid" };

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Should().NotContain(p => p.Name == "ProductId");
                properties.Should().NotContain(p => p.Name == "Rowguid");

                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }

        [Fact]
        public void SelectAllWithNameAndColorWithUnselectableProperties()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=Name,COLOR&top=0");
                _profile.UnselectableProperties = new[] { "color" };

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(1);

                properties.Should().Contain(p => p.Name == "Name");
                properties.Should().NotContain(p => p.Name == "COLOR");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectSkip50()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=ProductId,name,color&skip=50&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var first = query.First();
                var type = first.GetType();
                var value = int.Parse(type.GetProperty("ProductId").GetValue(first).ToString());

                value.Should().Be(51);
                query.Count().Should().Be(DataInitializer.ProductSampleCount - 50);
            }
        }

        [Fact]
        public void SelectTake50()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=ProductId,name,color&take=50");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                query.Count().Should().Be(50);
            }
        }

        [Fact]
        public void SelectSkipAndTake50()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=ProductId,name,color&skip=50&take=50");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var first = query.First();
                var type = first.GetType();
                var value = int.Parse(type.GetProperty("ProductId").GetValue(first).ToString());

                value.Should().Be(51);
                query.Count().Should().Be(50);
            }
        }

        [Fact]
        public void SelectOrderByColor()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,color&orderby=color&top=0");

                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>).ToList();
                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

                var type = first.GetType();
                var firstValue = type.GetProperty("color").GetValue(first).ToString();
                var secondValue = type.GetProperty("color").GetValue(second).ToString();

                var lastValue = type.GetProperty("color").GetValue(last).ToString();
                var preLastValue = type.GetProperty("color").GetValue(preLast).ToString();



                firstValue.Should().Be("black");
                secondValue.Should().Be("black");
                lastValue.Should().Be("red");
                preLastValue.Should().Be("red");
            }
        }

        [Fact]
        public void SelectOrderById()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=productid,name,color&orderby=productid&top=0");

                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>).ToList();

                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

                var type = first.GetType();
                var firstValue = int.Parse(type.GetProperty("productid").GetValue(first).ToString());
                var secondValue = int.Parse(type.GetProperty("productid").GetValue(second).ToString());

                var lastValue = int.Parse(type.GetProperty("productid").GetValue(last).ToString());
                var preLastValue = int.Parse(type.GetProperty("productid").GetValue(preLast).ToString());



                firstValue.Should().Be(1);
                secondValue.Should().Be(2);
                lastValue.Should().Be(DataInitializer.ProductSampleCount);
                preLastValue.Should().Be(DataInitializer.ProductSampleCount - 1);
            }
        }

        [Fact]
        public void SelectOrderByIdDesc()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=productid,name,color&orderby=-productid&top=0");

                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>).ToList();

                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

                var type = first.GetType();
                var firstValue = int.Parse(type.GetProperty("productid").GetValue(first).ToString());
                var secondValue = int.Parse(type.GetProperty("productid").GetValue(second).ToString());

                var lastValue = int.Parse(type.GetProperty("productid").GetValue(last).ToString());
                var preLastValue = int.Parse(type.GetProperty("productid").GetValue(preLast).ToString());


                lastValue.Should().Be(1);
                preLastValue.Should().Be(2);
                firstValue.Should().Be(DataInitializer.ProductSampleCount);
                secondValue.Should().Be(DataInitializer.ProductSampleCount - 1);
            }
        }

        [Fact]
        public void SelectOrderByColorDesc()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,color&orderby=-color&top=0");

                DataInitializer.InitializeSeed(context);
                var query = (context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>).ToList();
                var first = query.First();
                var second = query.Skip(1).First();

                var last = query.Last();
                var preLast = query.Skip(DataInitializer.ProductSampleCount - 2).First();

                var type = first.GetType();
                var firstValue = type.GetProperty("color").GetValue(first).ToString();
                var secondValue = type.GetProperty("color").GetValue(second).ToString();

                var lastValue = type.GetProperty("color").GetValue(last).ToString();
                var preLastValue = type.GetProperty("color").GetValue(preLast).ToString();

                firstValue.Should().Be("red");
                secondValue.Should().Be("red");
                lastValue.Should().Be("black");
                preLastValue.Should().Be("black");
            }
        }

        [Fact]
        public void SelectOrderBySellStartDate()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=SellStartDate&orderby=SellStartDate");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var currentDate = DateTime.MinValue;
                foreach (var product in query)
                {
                    var date = (DateTime)product.GetType().GetProperty("SellStartDate").GetValue(product);
                    date.Should().BeAfter(currentDate);
                    currentDate = date;
                }
            }
        }

        [Fact]
        public void SelectOrderBySellStartDateDesc()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=SellStartDate&orderby=-SellStartDate");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var currentDate = DateTime.MaxValue;
                foreach (var product in query)
                {
                    var date = (DateTime)product.GetType().GetProperty("SellStartDate").GetValue(product);
                    date.Should().BeBefore(currentDate);
                    currentDate = date;
                }
            }
        }

        [Fact]
        public void SelectFirst()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("first=true");

                DataInitializer.InitializeSeed(context);
                var product = context.Product.AutoQueryable(_autoQueryableContext) as object;
                var properties = product.GetType().GetProperties();
                properties.Should().Contain(p => p.Name == "ProductId");
                ((int)properties.First(p => p.Name == "ProductId").GetValue(product)).Should().Be(1);
            }
        }

        [Fact]
        public void SelectLast()
        {
            using (AutoQueryableDbContext context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("last=true");
                
                DataInitializer.InitializeSeed(context);
                var product = context.Product.AutoQueryable(_autoQueryableContext) as object;
                var properties = product.GetType().GetProperties();
                properties.Should().Contain(p => p.Name == "ProductId");
                ((int)properties.First(p => p.Name == "ProductId").GetValue(product)).Should().Be(1000);
            }
        }



        [Fact]
        public void SelectFirstOrderbyIdDesc()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("first=true&orderby=-productid");

                DataInitializer.InitializeSeed(context);
                var product = context.Product.AutoQueryable(_autoQueryableContext) as object;

                var properties = product.GetType().GetProperties();
                ((int)properties.First(p => p.Name == "ProductId").GetValue(product)).Should()
                    .Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllWithSelectInclude()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("top=50&select=name,SalesOrderDetail,productcategory");
                _profile.UnselectableProperties = new[] { "color" };

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext)  as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(3);
                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "SalesOrderDetail");
                properties.Should().Contain(p => p.Name == "productcategory");

                query.Count().Should().Be(50);
            }
        }

        [Fact]
        public void SelectWithIncludeNavigationProperties()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("top=50&select=name,SalesOrderDetail.Product.ProductId,productcategory");
                _profile.UnselectableProperties = new[] { "color" };

                DataInitializer.InitializeSeed(context);

                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var firstResult = query.First();
                var properties = firstResult.GetType().GetProperties();
                properties.Length.Should().Be(3);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "SalesOrderDetail").Which.Should().NotBeNull();
                properties.Should().Contain(p => p.Name == "productcategory");

                var salesOrderDetailProperty = properties.FirstOrDefault(p => p.Name == "SalesOrderDetail").PropertyType
                    .GenericTypeArguments[0];
                var productProperty = salesOrderDetailProperty.GetProperties().FirstOrDefault(x => x.Name == "Product");
                productProperty.Should().NotBeNull();
                productProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "ProductId");
                query.Count().Should().Be(50);
            }
        }



        [Fact]
        public void SelectAllProductsWithDtoProjection()
        {
            using (var context = new AutoQueryableDbContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.Select(p => new ProductDto
                {
                    Name = p.Name
                }).AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(1);

                properties.Should().Contain(p => p.Name == "Name");

                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithDtoProjectionAndSelectProjection()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,category.name&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.Select(p => new ProductDto
                {
                    Name = p.Name,
                    Category = new ProductCategoryDto
                    {
                        Name = p.ProductCategory.Name
                    }
                }).AutoQueryable(_autoQueryableContext) as IQueryable<object>;

                var properties = query.First().GetType().GetProperties();
                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "category");

                var categoryProperty = properties.FirstOrDefault(p => p.Name == "category");
                categoryProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "name");
                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithSelectProjectionWithUnselectableProperty()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,productcategory.name,ProductCategory.ProductCategoryId");
                _profile.UnselectableProperties = new[] { "ProductCategory.ProductCategoryId" };

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "productcategory");

                var productcategoryProperty = properties.FirstOrDefault(p => p.Name == "productcategory");
                productcategoryProperty.PropertyType.GetProperties().Should().Contain(x => x.Name == "name");
                productcategoryProperty.PropertyType.GetProperties().Should()
                    .NotContain(x => x.Name == "ProductCategoryId");
            }
        }

        [Fact]
        public void SelectAllProductsWithNameAndColorWithDtoProjection()
        {
            _queryStringAccessor.SetQueryString("select=name,color,categoryName&top=0");

            using (var context = new AutoQueryableDbContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.Select(p => new
                {
                    p.Name,
                    p.Color,
                    categoryName = p.ProductCategory.Name
                }).AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(3);

                properties.Should().Contain(p => p.Name == "name");
                properties.Should().Contain(p => p.Name == "color");
                properties.Should().Contain(p => p.Name == "categoryName");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithNameAndColorIgnoreCaseWithDtoProjection()
        {
            _queryStringAccessor.SetQueryString("select=Name,COLOR&top=0");
            
            using (var context = new AutoQueryableDbContext())
            {
                DataInitializer.InitializeSeed(context);
                var query = context.Product.Select(p => new
                {
                    p.Name,
                    p.Color,
                    categoryName = p.ProductCategory.Name
                }).AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(2);

                properties.Should().Contain(p => p.Name == "Name");
                properties.Should().Contain(p => p.Name == "COLOR");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllProductsWithUnselectablePropertiesWithDtoProjection()
        {
            using (var context = new AutoQueryableDbContext())
            {
                
                _profile.UnselectableProperties = new[] { "productid", "rowguid" };

                DataInitializer.InitializeSeed(context);
                var query = context.Product.Select(p => new
                {
                    p.ProductId,
                    p.Rowguid,
                    p.Name,
                    p.Color,
                    categoryName = p.ProductCategory.Name
                }).AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Should().NotContain(p => p.Name == "ProductId");
                properties.Should().NotContain(p => p.Name == "Rowguid");

                query.Count().Should().Be(DataInitializer.DefaultToTakeCount);
            }
        }


        [Fact]
        public void CountWithNullForeignKey()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=name,productextension.name&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                query?.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllPropertiesWithoutRelations()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=_&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(17);

                properties.Should().Contain(p => p.Name == "ProductId");
                properties.Should().Contain(p => p.Name == "Name");
                properties.Should().Contain(p => p.Name == "ProductNumber");
                properties.Should().Contain(p => p.Name == "Color");
                properties.Should().Contain(p => p.Name == "StandardCost");
                properties.Should().Contain(p => p.Name == "ListPrice");
                properties.Should().Contain(p => p.Name == "Size");
                properties.Should().Contain(p => p.Name == "Weight");
                properties.Should().Contain(p => p.Name == "ProductCategoryId");
                properties.Should().Contain(p => p.Name == "ProductModelId");
                properties.Should().Contain(p => p.Name == "SellStartDate");
                properties.Should().Contain(p => p.Name == "SellEndDate");
                properties.Should().Contain(p => p.Name == "DiscontinuedDate");
                properties.Should().Contain(p => p.Name == "ThumbNailPhoto");
                properties.Should().Contain(p => p.Name == "ThumbnailPhotoFileName");
                properties.Should().Contain(p => p.Name == "Rowguid");
                properties.Should().Contain(p => p.Name == "ModifiedDate");
                ;

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllPropertiesWithOneRelation()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=_,ProductModel&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(18);

                properties.Should().Contain(p => p.Name == "ProductId");
                properties.Should().Contain(p => p.Name == "Name");
                properties.Should().Contain(p => p.Name == "ProductNumber");
                properties.Should().Contain(p => p.Name == "Color");
                properties.Should().Contain(p => p.Name == "StandardCost");
                properties.Should().Contain(p => p.Name == "ListPrice");
                properties.Should().Contain(p => p.Name == "Size");
                properties.Should().Contain(p => p.Name == "Weight");
                properties.Should().Contain(p => p.Name == "ProductCategoryId");
                properties.Should().Contain(p => p.Name == "ProductModelId");
                properties.Should().Contain(p => p.Name == "SellStartDate");
                properties.Should().Contain(p => p.Name == "SellEndDate");
                properties.Should().Contain(p => p.Name == "DiscontinuedDate");
                properties.Should().Contain(p => p.Name == "ThumbNailPhoto");
                properties.Should().Contain(p => p.Name == "ThumbnailPhotoFileName");
                properties.Should().Contain(p => p.Name == "Rowguid");
                properties.Should().Contain(p => p.Name == "ModifiedDate");
                properties.Should().Contain(p => p.Name == "ProductModel");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }

        [Fact]
        public void SelectAllPropertiesFromLevelZero()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("select=*&top=0");

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(21);

                properties.Should().Contain(p => p.Name == "ProductId");
                properties.Should().Contain(p => p.Name == "Name");
                properties.Should().Contain(p => p.Name == "ProductNumber");
                properties.Should().Contain(p => p.Name == "Color");
                properties.Should().Contain(p => p.Name == "StandardCost");
                properties.Should().Contain(p => p.Name == "ListPrice");
                properties.Should().Contain(p => p.Name == "Size");
                properties.Should().Contain(p => p.Name == "Weight");
                properties.Should().Contain(p => p.Name == "ProductCategoryId");
                properties.Should().Contain(p => p.Name == "ProductModelId");
                properties.Should().Contain(p => p.Name == "SellStartDate");
                properties.Should().Contain(p => p.Name == "SellEndDate");
                properties.Should().Contain(p => p.Name == "DiscontinuedDate");
                properties.Should().Contain(p => p.Name == "ThumbNailPhoto");
                properties.Should().Contain(p => p.Name == "ThumbnailPhotoFileName");
                properties.Should().Contain(p => p.Name == "Rowguid");
                properties.Should().Contain(p => p.Name == "ModifiedDate");
                properties.Should().Contain(p => p.Name == "SalesOrderDetail");
                properties.Should().Contain(p => p.Name == "ProductExtension");
                properties.Should().Contain(p => p.Name == "ProductCategory");
                properties.Should().Contain(p => p.Name == "ProductModel");

                query.Count().Should().Be(DataInitializer.ProductSampleCount);
            }
        }
        [Fact]
        public async Task PagedResultTest()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("page=1");
                _profile.UseBaseType = true;

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<Product>;
                //var pagedResult = await query.ToPagedResultAsync(_autoQueryableContext);

                //pagedResult.RowCount.Should().Be(_profile.DefaultToTake);
                //pagedResult.TotalCount.Should().Be(await context.Product.CountAsync());
                //pagedResult.Result.Count.Should().Be(_profile.DefaultToTake);
                
            }
        }
        
        [Fact]
        public async Task DefaultToSelectAllTest()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("");
                _profile.DefaultToSelect = "*";

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(21);
                
            }
        }
        
        [Fact]
        public async Task DefaultToSelectBaseTest()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("nameContains=Product 1");
                _profile.DefaultToSelect = "_";

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(17);
                
            }
        }
        
        [Fact]
        public async Task DefaultToSelectTest()
        {
            using (var context = new AutoQueryableDbContext())
            {
                _queryStringAccessor.SetQueryString("nameContains=Product 1");
                _profile.DefaultToSelect = "productId,name";

                DataInitializer.InitializeSeed(context);
                var query = context.Product.AutoQueryable(_autoQueryableContext) as IQueryable<object>;
                var properties = query.First().GetType().GetProperties();

                properties.Length.Should().Be(2);
                
            }
        }

    }
}