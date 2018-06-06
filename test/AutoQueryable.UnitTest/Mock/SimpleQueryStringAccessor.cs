using AutoQueryable.Core.Models;

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
