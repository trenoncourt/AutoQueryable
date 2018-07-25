using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.QueryStringAccessors;

namespace AutoQueryable.UnitTest.Mock
{
    public class SimpleQueryStringAccessor : BaseQueryStringAccessor
    {
        public void SetQueryString(string queryString)
        {
            QueryString = queryString;
        }
    }
}
