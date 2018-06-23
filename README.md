# AutoQueryable &middot; [![NuGet](https://img.shields.io/nuget/v/AutoQueryable.svg?style=flat-square)](https://www.nuget.org/packages/AutoQueryable) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com) [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://github.com/trenoncourt/AutoQueryable/blob/master/LICENSE) [![Donate](	https://img.shields.io/beerpay/hashdog/scrapfy-chrome-extension.svg?style=flat-square)](https://www.paypal.me/trenoncourt/5)

> AutoQueryable add auto querying functionality like OData on top of IQueryable with best url practices. It help you to make requests like [http://baseurl/api/products?nameContains=frame&color=red,black](http://baseurl/api/products?nameContains=frame&color=red,black) with no effort.

## Installing / Getting started

| Package        | NuGet                                                                                     | |
|----------------|-------------------------------------------------------------------------------------------|-|
| Install-Package AutoQueryable   | [![NuGet Downloads](https://img.shields.io/nuget/dt/AutoQueryable.svg?style=flat-square)](https://www.nuget.org/packages/AutoQueryable) | Install without filters |
| AutoQueryable.AspNetCore.Filter | [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.AspNetCore.Filter.svg?style=flat-square)](https://www.nuget.org/packages/AutoQueryable.AspNetCore.Filter) | Install for **AspNet Core** |
| AutoQueryable.AspNetCore.Swagger      | [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.AspNetCore.Swagger.svg?style=flat-square)](https://www.nuget.org/packages/AutoQueryable.AspNetCore.Swagger) | Install for **AspNet Core** |
| AutoQueryable.AspNet.Filter     | [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.AspNet.Filter.svg?style=flat-square)](https://www.nuget.org/packages/AutoQueryable.AspNet.Filter) | Install for **Web api 2** |
| AutoQueryable.Nancy.Filter      | [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.Nancy.Filter.svg?style=flat-square)](https://www.nuget.org/packages/AutoQueryable.Nancy.Filter) | Install for **Nancy** |

**Other web framework?** You could made your own attribute, see **Use AutoQueryable without attribute** section.

## Basic usage

```c#
[Route("api/[controller]")]
public class ProductsController : Controller
{
    [HttpGet]
    [AutoQueryable]
    public IQueryable<Product> Get([FromServices] myDbContext dbContext)
    {
        return dbContext.Product;
    }
}
```

### For ASP.NET Core (using internal Dependency Injection)

```c#
[Route("api/[controller]")]
public class ProductsController : Controller
{
    [HttpGet]
    [TypeFilter(typeof(AutoQueryableAttribute))]
    public IQueryable<Product> Get([FromServices] myDbContext dbContext)
    {
        return dbContext.Product;
    }
}
```

With this url: [**/products?select=productId,name,color,productCategory.name,salesOrderDetail,salesOrderDetail.product**](/products?top=1&select=productId,name,color,productCategory.name,salesOrderDetail,salesOrderDetail.product)

You will get result like:

```json
[
    {
        "productId": 1,
        "name": "Product 0",
        "color": "red",
        "productCategory": {
            "name": "red"
        },
        "salesOrderDetail": [
            {
                "product": {
                    "productId": 1,
                    "name": "Product 0",
                    "productNumber": "24bb9446-d540-4513-a3c6-be4323984112",
                    "color": "red",
                    "standardCost": 1,
                    "listPrice": 0,
                    "size": "L",
                    "weight": 0,
                    "productCategoryId": 1,
                    "productModelId": 1,
                    "sellStartDate": "2017-07-14T00:00:00+02:00",
                    "rowguid": "f100e986-adeb-46b0-91de-9a1f8f3a4d11",
                    "modifiedDate": "0001-01-01T00:00:00",
                    "salesOrderDetail": []
                },
                "salesOrderId": 0,
                "salesOrderDetailId": 1,
                "orderQty": 5,
                "productId": 1,
                "unitPrice": 0,
                "unitPriceDiscount": 0,
                "lineTotal": 0,
                "rowguid": "00000000-0000-0000-0000-000000000000",
                "modifiedDate": "0001-01-01T00:00:00"
            },
            ...
        ]
    },
    ...
]

```

## Api URL usage

- Selectable columns: [/products?**select=name,color,toto**](/products?select=name,color,toto)
- Top/Take, Skip: [/products?**take=5&skip=5**](/products?take=5&skip=5)
- First, Last: [/products?**first=true**](/products?first=true)
- OrderBy, OrderByDesc: [/products?**orderby=price,id**](/products?orderby=price,id)
- Wrap with: [/products?wrapwith=**count,total-count,next-link**](/products?wrapwith=count,total-count,next-link)
- Filtering: [/products?**nameContains=frame&color=red,black**](/products?nameContains=frame&color=red,black) 
- Paging: [/products?**page=2&pagesize=10**](/products?page=2&pagesize=10) 

## Existing filters

By default filters are separated by AND (eg: color=red&color=black is translated by color == red AND color == black)

In a filter, comma separator is used for OR (eg: color=red,black is translated by color == red OR black)

- Equals '=': [/products?**color=red,black**](/products?color!=green,blue)
- Not Equals '!=': [/products?**color!=green,blue**](/products?color=red,black)
- Less Than, Greater Than '<', '>': [/products?**productCount\<5**](/products?productCount\<5)
- Less Than or Equals, Greater Than or equals '<=' [/products?**productCount\<=5**](/products?productCount\<=5)
- Contains 'contains': [/products?**colorContains=bla,ed**](/products?colorContains=bla,ed)
- StartsWith, EndsWith 'startswith', 'endswith': [/products?**colorStartsWith=bla,re**](/products?colorStartsWith=bla,re)

String filters have a negate (NOT) variant:

- NotContains 'notcontains': [/products?**colorContains!=bla,ed**](/products?colorContains!=bla,ed)
- NotStartsWith, NotEndsWith 'notstartswith', 'notendswith': [/products?**colorStartsWith!=bla,ed**](/products?colorContains!=bla,ed)

String filters have the following modifiers:

- **:i** (case insensitivity), ex. 'contains:i', 'startsWith:i!=': [/products?**colorContains:i=bla,ed**](/products?colorContains:i=bla,ed)

Filter aliases are not case sensitive:

- Contains 'contains': [/products?**colorcontains=bla,ed**](/products?colorcontains=bla,ed)

*Note that filters works with primitive types, string, datetime & guid*

### Filter modifiers

AQ provide property modifiers to modify property before filter, modifiers are denoted with **:**

- DateInYear <date-property>:year=<year>: [/products?**selldate:year=1989**](/products?selldate:year=1989)
- NotDateInYear <date-property>:year!=<year>: [/products?**selldate:year!=2000**](/products?selldate:year!=2000)

## Selection

- Select all properties of level zero without relations: [/products?**select=_**](/products?**select=_)
- Select all properties of level zero with relations: [/products?**select=\***](/products?**select=\*)
- Select an object with all its value types: [/products?**select=productcategory**](/products?select=productcategory)
- Select an object with all its values including navigation properties on the first level: [/products?**select=productcategory.\***](/products?select=productcategory.*)

## Projection

You can use projection in select & filters clauses with navigation properties (objects or collection of object)
- Select projection: [/products?**select=name,color,toto,productcategory.name**](/products?select=name,color,toto,productcategory.name)
- Filter projection: [/products?**salesorderdetail.product.productid=1**](/products?salesorderdetail.product.productid=1)

## Dto projection

You can still use dto projection and query over your dto with defined type:
```c#
[HttpGet]
[AutoQueryable]
public IQueryable Get([FromServices] AdventureWorksContext adventureWorksContext)
{
    return adventureWorksContext.Product.Select(p => new ProductProjection
    {
        Name = p.Name,
        ProductColor = p.Color,
        FinalPrice = p.price
    });
}
```

Or anonymous type:
```c#
[HttpGet]
[AutoQueryable]
public IQueryable Get([FromServices] AdventureWorksContext adventureWorksContext)
{
    return adventureWorksContext.Product.Select(p => new
    {
        p.Name,
        p.Color,
        FinalPrice = p.Price
    });
}
```

## Unselectable properties

If you want some properties to be unselectable (eg: Id, Password, ...)
```c#
[Route("api/[controller]")]
public class UsersController : Controller
{
    [HttpGet]
    [AutoQueryable(UnselectableProperties = new []{ "Password", "Id" })]
    public IQueryable<User> Get([FromServices] myDbContext dbContext)
    {
        return dbContext.User;
    }
}
```

*Note that Dto projection is the best way to limit selectable properties*

## Use base type instead of dynamic types

AQ uses dynamic types per default to ensure that the final flow will be as small as possible. In some cases you may not want to use dynamic types, there is a property for this:

```c#
[Route("api/[controller]")]
public class UsersController : Controller
{
    [HttpGet]
    [AutoQueryable(UseBaseType = true)]
    public IQueryable<User> Get([FromServices] myDbContext dbContext)
    {
        return dbContext.User;
    }
}
```

*Note that final flow will include null and default values but you can escape them with serializer settings for exemple*

## Use AutoQueryable without attribute 

If you don't want to use autoqueryable attribute you could use AQ directry in your code by passing it the querystring. 
First install the Autoqueryable package

```powershell
Install-Package AutoQueryable
```

```c#
[Route("api/[controller]")]
public class UsersController
{
    [HttpGet]
    public IActionResult Get([FromServices] myDbContext dbContext)
    {
        var resultSet = dbContext.Product.Select(p => new { ... }).AutoQueryable(queryString);
        return new OkObjectResult(new {
            foo = "",
            ....,
            Values = resultSet;
        })
    }
}
```

## Add Swagger parameters 

If you want to add AQ parameters to your swagger docs, just add AutoQueryable to the swagger conf.

```powershell
Install-Package AutoQueryable.AspNetCore.Swagger
```

```c#
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
    c.AddAutoQueryable(); // add this line
});
```

## AutoMapper

### ProjectTo()

If you are getting the error "Sequence contains no elements", you should use the package

```powershell
Install-Package AutoMapper.Extensions.ExpressionMapping
```

and replace the

```c#
ProjectTo<T>()
```

with

```c#
.UseAsDataSource().For<T>()
```

### Map relations

AutoQueryable and AutoMapper do not work together when it comes to mapping entity relations from an Entity to a Dto. To fix this, use the AutoQueryable profile setting

```c#
profile.ToListBeforeSelect = true
```

This performs a ToList() using the AutoMapper mapping before using the select query parameters and selecting only the properties asked by the Querystring