using System;
using System.Collections.Generic;
using System.Text;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.QueryStringAccessors;
using Microsoft.AspNetCore.Http;

namespace AutoQueryable.AspNetCore.Filter
{
    public class AspNetCoreQueryStringAccessor : BaseQueryStringAccessor
    {
        public AspNetCoreQueryStringAccessor(IHttpContextAccessor httpContextAccessor)
        {
            var queryString = httpContextAccessor.HttpContext.Request.QueryString.Value;

            QueryString = Uri.UnescapeDataString(queryString ?? "");

            _setQueryParts();
        }
    }
}
