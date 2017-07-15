# AutoQueryable
AutoQueryable add auto querying functionality like OData on top of IQueryable with best url practices. It help you to make requests like [http://baseurl/api/products?nameContains=frame&color=red,black](http://baseurl/api/products?nameContains=frame&color=red,black) with no effort.

## Installing / Getting started

Install without filters: [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.svg)](https://www.nuget.org/packages/AutoQueryable)
```powershell
Install-Package AutoQueryable.AspNetCore.Filter
```
Install for **AspNet Core**: [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.AspNetCore.Filter.svg)](https://www.nuget.org/packages/AutoQueryable.AspNetCore.Filter)
```powershell
Install-Package AutoQueryable.AspNetCore.Filter
```

Install for **Web api 2**: [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.AspNet.Filter.svg)](https://www.nuget.org/packages/AutoQueryable.AspNet.Filter)
```powershell
Install-Package AutoQueryable.AspNet.Filter
```

Install for **Nancy**: [![Nuget Downloads](https://img.shields.io/nuget/dt/AutoQueryable.Nancy.Filter.svg)](https://www.nuget.org/packages/AutoQueryable.Nancy.Filter)
```powershell
Install-Package AutoQueryable.Nancy.Filter
```

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

## Existing filters
By default filters are separated by AND (eg: color=red&color=black is translated by color == red AND color == black)

In a filter, comma separator is used for OR (eg: color=red,black is translated by color == red OR black)
- Equals '=': [/products?**color=red,black**](/products?color!=green,blue)
- Not Equals '!=': [/products?**color!=green,blue**](/products?color=red,black)
- Less Than, Greater Than '<', '>': [/products?**productCount\<5**](/products?productCount\<5)
- Less Than or Equals, Greater Than or equals '<=' [/products?**productCount\<=5**](/products?productCount\<=5)
- Contains 'contains': [/products?**colorContains=bla,ed**](/products?colorContains=bla,ed)
- StartsWith, EndsWith 'startswith', 'endswith': [/products?**colorStartsWith=bla,re**](/products?colorStartsWith=bla,re)

*Note that filters works with primitive types, string, datetime & guid*

## Selection
- Select an object with all its value types: [/products?**select=productcategory**](/products?select=productcategory)
- Select an object with all its values including navigation properties on the first level: [/products?**select=productcategory.\***](/products?select=productcategory.*)

## Projection
You can use projection in select & filters clauses with navigation properties (objects or collection of object)
- Select projection: [/products?**select=name,color,toto,productcategory.name**](/products?select=name,color,toto,productcategory.name)
- Filter projection: [/products?**salesorderdetail.product.productid=1**](/products?salesorderdetail.product.productid=1)

## Dto projection
```c#
[HttpGet]
[AutoQueryable]
public IQueryable Get([FromServices] AdventureWorksContext adventureWorksContext)
{
    return adventureWorksContext.Product.Select(p => new ProductProjection
    {
        Name = p.Name
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

Roadmap :
- ~~Add **Top**, **Skip**, **Take**, **OrderBy** keywords~~
- ~~Add capability to include navidation properties (aka expand in OData)~~
- ~~Add capability to select properties (columns in table)~~
- ~~Add capability to make projection on entities~~
- ~~Add capability to get single element (first or last)~~
- ~~Add unselectable columns~~
- ~~Use Expression tree instead of DbSet\<T>.FromSql for filters~~
- ~~Add EntityFramework 6.1.3 support~~
- ~~Add .Net 4.5.1 min support~~
- ~~Add capability to select with projection~~
- ~~Add simpler Attribute using OnActionExecuted~~
- ~~Add Samples~~
- ~~Add capability to use Dto projection~~
- ~~Add capability to include hierarchical data~~
- ~~Add capability to include navigation properties on multiple levels~~
- Add allowed clauses or not
- Add allowed operators or not in where clause
- Add maximum value on clauses (eg maximum top 999 or maximum depth 2)
- Add Demo
- Add Unselectable navigations in include clause
- Add more date filters in where clause eg: yearEquals
- Add capability to choose to ignore case or not (case is ignored for now)
- Add capability to use Group by
- Add capability to set AutoQueryable options in headers
- Add an option to not use dynamic objects (Use the type T provided by the IQueryable<T>)
- Add .* selector to select all properties including navigation properties on the first level
- Add Odata-v(x) & GraphQL as plugin (choose beetween AutoQueryable, Odata or GraphQL for query)

## Buy me a beer
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/trenoncourt/5)
