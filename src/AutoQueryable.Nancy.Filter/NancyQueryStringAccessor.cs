using AutoQueryable.Core.Models.QueryStringAccessors;

namespace AutoQueryable.Nancy.Filter
{
    public class NancyQueryStringAccessor : BaseQueryStringAccessor
    {
        public void SetQueryString(string queryString)
        {
            QueryString = queryString;
        }
    }
}
