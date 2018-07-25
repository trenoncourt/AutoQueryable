using AutoQueryable.Core.Models.QueryStringAccessors;

namespace AutoQueryable.Core.Models
{
    public class TestQueryStringAccessor : BaseQueryStringAccessor
    {
        public TestQueryStringAccessor(string queryString)
        {
            QueryString = queryString;
        }
    }
}
