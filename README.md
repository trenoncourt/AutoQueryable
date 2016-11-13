# AutoQueryable
AutoQueryable add auto querying functionality like OData with best url practices to Asp.Net Core

Install with [NuGet](https://www.nuget.org/packages/AutoQueryable):
```powershell
Install-Package AutoQueryable
```

Basic usage:
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
Or you can use TypeFilterAttribute from Asp.Net Core:
```c#
[Route("api/[controller]")]
public class ProductsController : Controller
{
    [HttpGet]
    [TypeFilter(typeof(AutoQueryableFilterAttribute<AdventureWorksContext, Product>))]
    public void Get()
    {
    }
}
```


Then you can make requests like http://baseurl/api/products?nameContains=frame&color=red,black

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
Or with TypeFilter :
```c#
[Route("api/[controller]")]
public class ProductsController : Controller
{
    [HttpGet]
    [TypeFilter(typeof(AutoQueryableFilterAttribute<AdventureWorksContext, Product>), Arguments = new object[] { true })]
    public IActionResult Get()
    {
        return Ok(_dbContext.Products);
    }
}
```

Roadmap :
- Add **Top**, **Skip**, **Take**, **OrderBy** keywords
- Add capability to include navidation properties (aka expand in OData)
- Add capability to select properties (columns in table)
- Add capability to choose which property (column in table) can be filtered
