# AutoQueryable
AutoQueryable add auto querying functionality like OData on top of IQueryable with best url practices to Asp.Net & Asp.Net Core.

Install for **AspNet Core** [NuGet](https://www.nuget.org/packages/AutoQueryable.AspNetCore.Filter/):
```powershell
Install-Package AutoQueryable.AspNetCore.Filter
```

Install for **Web api 2** [NuGet](https://www.nuget.org/packages/AutoQueryable.AspNet.Filter/):
```powershell
Install-Package AutoQueryable.AspNet.Filter
```

AutoQueryable helps you to make requests like [http://baseurl/api/products?nameContains=frame&color=red,black](http://baseurl/api/products?nameContains=frame&color=red,black)

From version 0.2.0 you can use selectable columns eg: [http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto)

From version 0.3.1 you can use Top/Take, Skip keywords eg: [http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5)

From version 0.4.0 you can now use First or Last keyword to select only one element eg:
[http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5&first=true](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5&first=true)

From version 0.6.0 you can now use OrderBy or OrderByDesc keyword to order by one or more elements eg:
[http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5&first=true&orderby=price,id](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5&first=true&orderby=price,id)

From version 0.7.0 you can now use Include keyword to include navigation properties from first level eg:
[http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&include=category](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&include=category)

From version 0.10.0 you can use Projection in Select clause :
[http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto,productcategory.name](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto,productcategory.name)

**Existing filters** 

By default filters are separated by AND (eg: color=red&color=black is translated by color == red AND color == black)

In a filter, comma separator is used for OR (eg: color=red,black is translated by color == red OR black)
- Equals '=' (eg color=red or color=red,black)
- Not Equals '!=' (eg color!=green or color!=green,blue)
- Less Than '<' (eg productCount\<5)
- Less Than or Equals '<=' (eg productCount\<=5)
- Greater Than '>' (eg productCount>5)
- Greater Than or Equals '>=' (eg productCount>=5)
- Contains 'contains' (eg colorContains=ed or colorContains=bla,ed)
- StartsWith 'startswith' (eg colorStartsWith=re or colorStartsWith=bla,re)
- EndsWith 'endswith' (eg colorEndsWith=ed or colorEndsWith=ack,ed)

**Basic usage**
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

**Dto projection**
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
*Note that with Dto projection, you cannot use include keyword*

**Unselectable properties** 
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
- Add Samples
- Add Demo
- Add capability to include navigation properties on multiple levels
- Add Unselectable navigations in include clause
- Add allowed clauses or not
- Add allowed operators or not in where clause
- Add more date filters in where clause eg: yearEquals
- Add capability to use Dto projection
- Add capability to choose to ignore case or not (case is ignored for now)
- Add capability to use Group by
- Add capability to include hierarchical data
