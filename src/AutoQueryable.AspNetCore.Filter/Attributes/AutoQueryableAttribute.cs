using System;
using AutoQueryable.AspNetCore.Filter.Filters;
using AutoQueryable.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoQueryable.AspNetCore.Filter.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AutoQueryableAttribute : Attribute, IFilterFactory, IOrderedFilter
    {
        public string[] UnselectableProperties { get; set; }

        /// <inheritdoc />
        public bool IsReusable { get; set; }

        /// <inheritdoc />
        public int Order { get; set; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }
            var profile = new AutoQueryableProfile
            {
                UnselectableProperties = UnselectableProperties
            };
            
            return new AutoQueryableFilter(profile);
        }
    }
}
