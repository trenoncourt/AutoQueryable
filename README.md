# AutoQueryable
AutoQueryable add auto querying functionality like OData with best url practices to Asp.Net Core

Install with [NuGet](https://www.nuget.org/packages/AutoQueryable):
```powershell
Install-Package AutoQueryable
```

AutoQueryable helps you to make requests like [http://baseurl/api/products?nameContains=frame&color=red,black](http://baseurl/api/products?nameContains=frame&color=red,black)

From version 0.2.0 you can use selectable columns eg: [http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto)

From version 0.3.1 you can use Top/Take, Skip keywords eg: [http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5](http://baseurl/api/products?nameContains=frame&color=red,black&select=name,color,toto&take=5&skip=5)

**Basic usage**
```c#
[Route("api/[controller]")]
public class ProductsController : Controller
{
    [HttpGet]
    [AutoQueryable(DbContextType = typeof(AdventureWorksContext), EntityType = typeof(Product))]
    public void Get()
    {
    }
}
```

**Unselectable properties** 
If you want some properties to be unselectable (eg: Id, Password, ...)
```c#
[Route("api/[controller]")]
public class UsersController : Controller
{
    [HttpGet]
    [AutoQueryable(DbContextType = typeof(AdventureWorksContext), EntityType = typeof(User), UnselectableProperties = new []{ "Password", "Id" })]
    public void Get()
    {
    }
}
```

**Fallback value** 
Sometimes filters can cause exceptions, you can specify you want to use a fallback value to hide exception eg:
```c#
[Route("api/[controller]")]
public class ProductsController : Controller
{
    [HttpGet]
    [AutoQueryable(DbContextType = typeof(AdventureWorksContext), EntityType = typeof(Product), UseFallbackValue = true)]
    public IActionResult Get()
    {
        return Ok(_dbContext.Products);
    }
}
```

Roadmap :
- Add ~~**Top**, **Skip**, **Take**~~, **OrderBy** keywords
- Add capability to include navidation properties (aka expand in OData)
- ~~Add capability to select properties (columns in table)~~
- ~~Add capability to make projection on entities~~
- Add capability to get single element
- ~~Add unselectable columns~~
- Add capability to use Dto projection
