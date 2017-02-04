using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace AutoQueryable.Sample.EfCore.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController
    {
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
