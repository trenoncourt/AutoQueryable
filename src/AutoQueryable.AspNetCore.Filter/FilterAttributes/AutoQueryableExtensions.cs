using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoQueryable.Core.Models;
using Microsoft.AspNetCore.Http;

namespace AutoQueryable.AspNetCore.Filter.FilterAttributes
{
    public class AutoQueryable
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AutoQueryable(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public dynamic AutoQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class
        {
            var context = new AutoQueryableContext<TEntity>("");//_httpContextAccessor.HttpContext.Request.GetUri()
            return context.GetAutoQuery(query);
        }
    }
}
