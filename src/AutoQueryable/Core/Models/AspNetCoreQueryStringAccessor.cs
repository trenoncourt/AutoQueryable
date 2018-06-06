using System;
using Microsoft.AspNetCore.Http;

namespace AutoQueryable.Core.Models
{
    public class AspNetCoreQueryStringAccessor : BaseQueryStringAccessor, IQueryStringAccessor
    {
        public AspNetCoreQueryStringAccessor(IHttpContextAccessor httpContextAccessor)
        {
            var queryString = httpContextAccessor.HttpContext.Request.QueryString.Value;

            QueryString = Uri.UnescapeDataString(queryString ?? "");

            _setQueryParts();
        }
    }
}