using System;
using System.Collections.Generic;
using System.Text;
using AutoQueryable.Core.Models;
using Nancy;

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
