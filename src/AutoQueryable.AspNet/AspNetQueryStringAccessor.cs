using System.Web;
using AutoQueryable.Core.Models.QueryStringAccessors;

namespace AutoQueryable.AspNet
{
    public class AspNetQueryStringAccessor : BaseQueryStringAccessor
    {
        public AspNetQueryStringAccessor()
        {
            QueryString = HttpContext.Current.Request.ServerVariables["query_string"];
        }
    }
}